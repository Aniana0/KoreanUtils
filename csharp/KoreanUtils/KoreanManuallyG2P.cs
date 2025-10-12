namespace KoreanUtils
{
    public static class KoreanManuallyG2P
    {
        private static readonly ExceptionWordYaml _exceptionWordYaml;
        private static readonly JamoPropsYaml jamoProps;
        private static readonly KoreanRuleYaml rules;
        private static Dictionary<string, string> ExceptionWords => _exceptionWordYaml.ExceptionWords;

        static KoreanManuallyG2P()
        {
            try
            {
                _exceptionWordYaml = BaseDataLoader.LoadBaseYaml<ExceptionWordYaml>("KoreanUtils.exception_word.yaml");
                jamoProps = BaseDataLoader.LoadBaseYaml<JamoPropsYaml>("KoreanUtils.jamo_propertie.yaml");
                rules = BaseDataLoader.LoadBaseYaml<KoreanRuleYaml>("KoreanUtils.korean_rule.yaml");
                if (_exceptionWordYaml == null || jamoProps == null) throw new Exception($"Load Failed : Exception Words is null");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Load Failed : Exception Words, {ex}");
                throw;
            }
        }

        public static (string result, string? matchWord, int? matchIndex) G2pFromException(string text, int targetStartIndex, int targetLenth = 1, Dictionary<string, string>? ExceptionDict = null)
        {
            ExceptionDict ??= ExceptionWords;
            int targetEndIndex = targetStartIndex + targetLenth;
            foreach (string word in ExceptionDict.Keys)
            {
                int sliceStartIndex = targetEndIndex - (word.Length - 1);
                int sliceEndIndex = targetStartIndex + (word.Length - 1);
                if (sliceStartIndex < 0) sliceStartIndex = 0;
                if (sliceEndIndex > text.Length) sliceEndIndex = text.Length;
                if (targetEndIndex > sliceEndIndex) continue;
                string textSelection = text[sliceStartIndex..sliceEndIndex];
                if (textSelection.Contains(word))
                {
                    int targetIndexOfWord = targetStartIndex - textSelection.IndexOf(word);
                    return (ExceptionDict[word][targetIndexOfWord..(targetIndexOfWord + targetLenth)], word, targetIndexOfWord);
                }
            }
            return (text[targetStartIndex..targetEndIndex], null, null);
        }

        public static char CharG2p(char character, string? prev="", string? next="")
        {
            if (!HangeulUtils.IsHangeul(character)) return character;
            // 예외 단어면 사전에 등록된 발음 적용
            var (result, matchWord, matchIndex) = prev != null ? G2pFromException($"{prev}{character}{next}", prev.Length) : G2pFromException($"{character}{next}", 0);
            bool isExceptionWord = matchWord != null;
            if (isExceptionWord && matchIndex != 0 && matchIndex != matchWord?.Length) return result[0];
            
            bool isStartOfExceptionWord = isExceptionWord && matchIndex == 0;
            bool isEndOfExceptionWord = isExceptionWord && matchIndex == matchWord?.Length;

            // 자모 분리
            char originChar = result[0];
            var originJamo = HangeulUtils.ToJamo(originChar);

            if (originJamo == null) return character;
            
            // 앞 뒤 한 글자만 남기기
            char? prevChar = null;
            char? nextChar = null;
            if (!isStartOfExceptionWord && prev?.Length > 0) prevChar = prev[^1];
            if (!isEndOfExceptionWord && next?.Length > 0) nextChar = next[0];
            var prevJamo = prevChar != null && HangeulUtils.IsHangeul(prevChar) ? HangeulUtils.ToJamo(prevChar.Value) : null;
            var nextJamo = nextChar != null && HangeulUtils.IsHangeul(nextChar) ? HangeulUtils.ToJamo(nextChar.Value) : null;

            char currStartC = originJamo.Value.Choseong;
            char currV = originJamo.Value.Jungseong;
            char? currFinalC = originJamo?.Jongseong;

            // 초성 작업 시작
            if (prevJamo?.Jongseong != null)
            {
                char prevC = prevJamo.Value.Jongseong.Value;
                char prevCSound = jamoProps.Consonant[prevC]?.Forms?.Default ?? prevC;

                var currTags = jamoProps.Consonant[currStartC].Tag;
                var currForms = jamoProps.Consonant[currStartC].Forms;
                var prevTags = jamoProps.Consonant[prevC].Tag;
                var prevForms = jamoProps.Consonant[prevC].Forms;
                var prevSoundTags = jamoProps.Consonant[prevCSound].Tag;
                var prevSoundForms = jamoProps.Consonant[prevCSound].Forms;

                if (prevSoundTags.Contains("n-to-l") && currStartC == 'ㄴ') currStartC = prevForms?.Liquid ?? currStartC;
                else if (prevSoundTags.Contains("l-to-n") && currStartC == 'ㄹ') currStartC = prevForms?.Nasal ?? currStartC;
                else if (currTags.Contains("plain"))
                {
                    if (prevSoundTags.Contains("plain-to-tense")) currStartC = currForms?.Tense ?? currStartC;
                    else if (prevSoundTags.Contains("plain-to-aspirated")) currStartC = currForms?.Aspirated ?? currStartC;
                }
                else if (currTags.Contains("silent"))
                {
                    if (prevSoundTags.Contains("single") || prevSoundTags.Contains("double")) currStartC = prevC;
                    else if (prevSoundTags.Contains("mix")) currStartC = prevForms?.Next == 'ㅅ' ? 'ㅆ' : prevForms?.Next ?? currStartC;
                    if (currV == 'ㅣ' && prevTags.Contains("palatalization")) currStartC = prevForms?.Palatalization ?? currStartC;
                }
                else if (currStartC == 'ㅎ')
                {
                    if (prevTags.Contains("double") || prevSoundTags.Contains("mix")) currStartC = prevForms?.Next != null ? jamoProps.Consonant[prevForms.Next.Value].Forms?.Aspirated ?? currStartC : currStartC;
                    else if (prevTags.Contains("single") && prevSoundTags.Contains("plain")) currStartC = prevSoundForms?.Aspirated ?? currStartC;
                    if (currV == 'ㅣ' && prevTags.Contains("palatalization")) currStartC = 'ㅊ';
                }
            }

            // 중성 작업 시작
            if (!jamoProps.Consonant[currStartC].Tag.Contains("silent") && currV == 'ㅢ') currV = 'ㅣ';
            else if (jamoProps.Consonant[currStartC].Tag.Contains("del-y") && jamoProps.Vowel[currV].Tag.Contains("group-y")) currV = jamoProps.Vowel[currV].MainVowel;

            // 종성 작업 시작
            if (currFinalC != null && nextJamo?.Choseong != null)
            {
                char currC = currFinalC.Value;
                char nextC = nextJamo.Value.Choseong;
                var currTags = jamoProps.Consonant[currC].Tag;
                var currForms = jamoProps.Consonant[currC].Forms;
                var nextTags = jamoProps.Consonant[nextC].Tag;
                // 밟, 넓 확인
                foreach (string filter in rules.ExceptionJongseong11)
                {
                    if ($"{originChar}{nextChar}" == filter || $"{originChar}" == filter) currFinalC = 'ㅂ';
                }
                bool isNextPlain = (currC == nextC || currC == 'ㅎ') && nextTags.Contains("plain");
                bool isMove = (currTags.Contains("single") || currTags.Contains("double") || currC == 'ㅀ' || currC == 'ㄶ') && nextTags.Contains("silent");
                bool isMixH = currTags.Contains("single") && nextC == 'ㅎ';
                if (isNextPlain || isMove || isMixH) currFinalC = null;
                else if (currTags.Contains("mix") && (nextC == 'ㅇ' || nextC == 'ㅎ')) currFinalC = currForms?.Prev;
                else currFinalC = currForms?.Default;
                if (currFinalC != null && nextTags.Contains("jongseong-to-nasal")) currFinalC = jamoProps.Consonant[currFinalC.Value].Forms?.Nasal;
                if (currFinalC == 'ㄴ' && nextC == 'ㄹ') currFinalC = jamoProps.Consonant[currFinalC.Value].Forms?.Liquid;
            }
            return HangeulUtils.ToHangeulChar(currStartC, currV, currFinalC) ?? character;
        }
    }
}