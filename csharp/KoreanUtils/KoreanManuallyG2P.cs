namespace KoreanUtils
{
    public static class KoreanManuallyG2P
    {
        private static readonly ExceptionWordYaml _exceptionWordYaml;
        private static IReadOnlyDictionary<string, string> ExceptionWords => _exceptionWordYaml.ExceptionWords;

        static KoreanManuallyG2P()
        {
            try
            {
                _exceptionWordYaml = BaseDataLoader.LoadBaseYaml<ExceptionWordYaml>("KoreanUtils.exception_word.yaml");
                if (_exceptionWordYaml == null) throw new Exception($"Load Failed : Exception Words is null");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Load Failed : Exception Words, {ex}");
                throw;
            }
        }

        public static char CharG2p(string text, int targetIndex, Dictionary<string, string> ExceptionDict)
        {
            foreach (string word in ExceptionDict.Keys)
            {
                if (word.Length - 1 <= targetIndex) 
            }
        }

        public static char CharG2p(char character, string? prev="", string? next="")
        {
            if (!HangeulUtils.IsHangeul(character)) return character;
            // 예외 단어면 사전에 등록된 발음 적용
            foreach (string word in ExceptionWords.Keys)
            {
                if (prev?.Length >= word.Length) prev = $"{prev[^(word.Length - 1)..]}";
                if (next?.Length >= word.Length) next = $"{next[..(word.Length - 1)]}";
                string fullText = $"{prev}{character}{next}";
                if (fullText.Contains(word))
                {
                    int wordStartIndex = fullText.IndexOf(word);
                    int wordEndIndex = wordStartIndex + word.Length;
                    while (true)
                    {
                        if (wordStartIndex <= prev?.Length && prev.Length < wordEndIndex) break;
                        wordStartIndex = fullText.IndexOf(word, wordStartIndex + 1);
                        if (wordStartIndex == -1) throw new InvalidOperationException($"Error.");
                        wordEndIndex = wordStartIndex + word.Length;
                    }
                    int charIndex = prev.Length - wordStartIndex;

                    if (0 < charIndex && charIndex < word.Length) return ExceptionWords[word][charIndex];

                    character = ExceptionWords[word][charIndex];
                }
            }
            // 자모 분리
            
            // 앞 뒤 한 글자만 남기기
            char? prevChar = null;
            char? nextChar = null;
            if (prev?.Length > 0) prevChar = prev[^1];
            if (next?.Length > 0) nextChar = next[0];
            var prevJamo = prevChar != null ? HangeulUtils.ToJamo(prevChar.Value) : null;
            var nextJamo = nextChar != null ? HangeulUtils.ToJamo(nextChar.Value) : null;

            return character;
        }
    }
}