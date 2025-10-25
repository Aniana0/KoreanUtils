namespace KoreanUtils
{
    public static class KoreanManuallyG2P
    {
        public static Dictionary<string, string> ExceptionWords => BaseDatas.ExceptionWords;
        public static Dictionary<char, ConsonantProps> CProps => BaseDatas.CProps;
        public static Dictionary<char, VowelProps> VProps => BaseDatas.VProps;
        public static HashSet<string> ExceptionLB => BaseDatas.ExceptionLB;

        //  예외 단어 판별
        public static string ApplyExceptionDict(string text, Dictionary<string, string>? ExceptionDict = null)
        {
            ExceptionDict ??= ExceptionWords;
            string convertText = text;
            foreach (var word in ExceptionDict.Keys.OrderByDescending(k => k.Length)) convertText = convertText.Replace(word, ExceptionDict[word]);
            return convertText ?? text;
        }

        private static char ProcessChoseong(char currChoseong, char prevJongseong, HashSet<string> currChoseongTags, ConsonantForms? currChoseongForms, char currJungseong)
        {
            char prevSound = CProps[prevJongseong].Forms?.Default ?? throw new Exception($"[Jamo Props Error] : {prevJongseong} has no default form.");

            var prevTags = CProps[prevJongseong].Tag;
            var prevForms = CProps[prevJongseong].Forms;
            var prevSoundTags = CProps[prevSound].Tag;
            var prevSoundForms = CProps[prevSound].Forms;

            return currChoseong switch
            {
                'ㄴ' when prevSoundTags.Contains("n-to-l") => prevForms?.Liquid,
                'ㄹ' when prevSoundTags.Contains("l-to-n") => prevForms?.Nasal,
                _ when currChoseongTags.Contains("plain") => prevTags switch
                {
                    var tags when tags.Contains("plain-to-tense") => currChoseongForms?.Tense,
                    var tags when tags.Contains("plain-to-aspirated") => currChoseongForms?.Aspirated,
                    _ => currChoseong
                },
                _ when currChoseongTags.Contains("silent") => prevTags switch
                {
                    var tags when currJungseong == 'ㅣ' && tags.Contains("palatalization") => prevForms?.Palatalization,
                    var tags when tags.Contains("single") || tags.Contains("double") => prevJongseong,
                    var tags when tags.Contains("mix") => prevForms?.Next == 'ㅅ' ? 'ㅆ' : prevForms?.Next,
                    _ => currChoseong
                },
                'ㅎ' => prevTags switch
                {
                    var tags when currJungseong == 'ㅣ' && tags.Contains("palatalization") => 'ㅊ',
                    var tags when tags.Contains("single") && prevSoundTags.Contains("plain") => prevSoundForms?.Aspirated,
                    var tags when tags.Contains("double") || tags.Contains("mix") => prevForms?.Next != null ? CProps[prevForms.Next.Value].Forms?.Aspirated : throw new Exception($"[Jamo Props Error] : {prevJongseong} has no next form."),
                    _ => currChoseong
                },
                _ => currChoseong
            } ?? currChoseong;
        }

        private static char ProcessJungseong(char currJungseong, HashSet<string> currChoseongTags, HashSet<string> currJungseongTags, char currJungseongVowel)
        {
            return currJungseong switch
            {
                'ㅢ' when !currChoseongTags.Contains("silent") => 'ㅣ',
                _ when currJungseongTags.Contains("group-y") && currChoseongTags.Contains("del-y") => currJungseongVowel,
                _ => currJungseong
            };
        }
        
        private static char? ProcessJongseong(char? nextChoseong, char currJongseong, char currChar, char? nextChar)
        {
            var currJongseongTags = CProps[currJongseong].Tag;
            var currJongseongForms = CProps[currJongseong].Forms;
            var nextTags = nextChoseong != null ? CProps[nextChoseong.Value].Tag : null;

            // 밟, 넓 확인
            string currPair = $"{currChar}{nextChar}";
            if (ExceptionLB.Contains(currPair) || ExceptionLB.Contains(currChar.ToString())) currJongseong = 'ㅂ';

            if (nextTags != null)
            {
                bool isJoinAspirated = (currJongseong == 'ㅎ' && nextTags.Contains("plain")) || (currJongseongTags.Contains("single") && nextChoseong == 'ㅎ');
                bool isJoinPlain = currJongseong == nextChoseong && nextTags.Contains("plain");
                bool isMoveNext = (currJongseongTags.Contains("single") || currJongseongTags.Contains("double") || currJongseongTags.Contains("plain-to-aspirated")) && nextTags.Contains("silent");
                bool isDeleteJongseong = isJoinAspirated || isJoinPlain || isMoveNext;

                if (isDeleteJongseong) return null;
                else if (currJongseongTags.Contains("mix") && (nextChoseong == 'ㅇ' || nextChoseong == 'ㅎ')) currJongseong = currJongseongForms?.Prev ?? currJongseong;
                
                currJongseong = currJongseongForms?.Default ?? throw new Exception($"[Jamo Props Error] : {currJongseong} has no default form.");
                currJongseongForms = CProps[currJongseong].Forms;
                
                if (nextTags.Contains("jongseong-to-nasal")) currJongseong = currJongseongForms?.Nasal ?? currJongseong;
                if (currJongseong == 'ㄴ' && nextTags.Contains("n-to-l")) currJongseong = currJongseongForms?.Liquid ?? currJongseong;
            }

            return CProps[currJongseong].Forms?.Default;
        }

        public static (char Character, (char Choseong, char Jungseong, char? Jongseong)? Jamo) CharG2p(char currChar, char? prevChar = null, char? nextChar = null)
        {
            // 자모 분리
            var currJamo = HangeulUtils.ToJamo(currChar);
            if (currJamo == null) return (currChar, null);

            // 앞 뒤 한 글자만 남기기
            var prevJamo = prevChar != null ? HangeulUtils.ToJamo(prevChar.Value) : null;
            var nextJamo = nextChar != null ? HangeulUtils.ToJamo(nextChar.Value) : null;

            char currChoseong = currJamo.Value.Choseong;
            char currJungseong = currJamo.Value.Jungseong;
            char? currJongseong = currJamo?.Jongseong;

            var currChoseongTags = CProps[currChoseong].Tag;
            var currChoseongForms = CProps[currChoseong].Forms;
            var currJungseongTags = VProps[currJungseong].Tag;
            var currJungseongVowel = VProps[currJungseong].MainVowel;

            // 초성 작업 시작
            if (prevJamo?.Jongseong != null) currChoseong = ProcessChoseong(currChoseong, prevJamo.Value.Jongseong.Value, currChoseongTags, currChoseongForms, currJungseong);
            
            // 중성 작업 시작
            currJungseong = ProcessJungseong(currJungseong, currChoseongTags, currJungseongTags, currJungseongVowel);

            // 종성 작업 시작
            if (currJongseong != null) currJongseong = ProcessJongseong(nextJamo?.Choseong, currJongseong.Value, currChar, nextChar);

            currChar = HangeulUtils.ToHangeulChar(currChoseong, currJungseong, currJongseong) ?? currChar;

            return (currChar, (currChoseong, currJungseong, currJongseong));
        }
    }
}