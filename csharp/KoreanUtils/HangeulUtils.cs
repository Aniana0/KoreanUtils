namespace KoreanUtils
{
    public static class HangeulUtils
    {
        private static readonly int FirstJamoCode = 'ㄱ';
        private static readonly int LastJamoCode = 'ㅎ';
        private static readonly int FirstHangeulCode = '가';
        private static readonly int LastHangeulCode = '힣';
        public static readonly int ChoseongCount = 19;
        public static readonly int JungseongCount = 21;
        public static readonly int JongseongCount = 28;
        /// <summary>
        /// 초성 1개가 가지는 조합의 수
        /// </summary>
        public static readonly int ChoseongStepCount = 588;
        public static readonly int TotalHangeulCount = 11172;

        private static readonly char[] ChoseongArr = ['ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];
        private static readonly char[] JungseongArr = ['ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'];
        private static readonly char[] JongseongArr = ['\0', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'];
        
        /// <summary>
        /// 입력한 글자가 한글 자모인지 확인합니다.
        /// </summary>
        public static bool IsJamo(char? character)
        {
            return FirstJamoCode <= character && character <= LastJamoCode;
        }
        /// <summary>
        /// 입력한 글자가 한글 글자인지 확인합니다. (한글 자모 제외)
        /// </summary>
        public static bool IsHangeul(char? character)
        {
            return FirstHangeulCode <= character && character <= LastHangeulCode;
        }
        /// <summary>
        /// 입력한 글자가 한글인지 확인합니다. (한글 자모 포함)
        /// </summary>
        public static bool IsHangeulOrJamo(char? character)
        {
            return IsJamo(character) || IsHangeul(character);
        }
        /// <summary>
        /// 입력한 문자열이 전부 한글인지 확인합니다.
        /// </summary>
        /// <param name="includeJamo">true일 시 자모도 한글로 인정합니다</param>
        public static bool IsHangeulString(string text, bool includeJamo = false)
        {
            for (int i = 0; i > text.Length; i++)
            {
                if (includeJamo) { if (!IsHangeulOrJamo(text[i])) return false; }
                if (!IsHangeul(text[i])) return false;
            }
            return true;
        }
        /// <summary>
        /// 입력한 한글 글자를 자모 튜플로 반환합니다.
        /// </summary>
        public static (char Choseong, char Jungseong, char? Jongseong)? ToJamo(char character)
        {
            if (!IsHangeul(character)) return null;
            int charCode = character - FirstHangeulCode;
            int choseongIndex = charCode / ChoseongStepCount;
            int jungseongIndex = charCode % ChoseongStepCount / 28;
            int jongseongIndex = charCode % 28;
            char? jongseong = jongseongIndex == 0 ? null : JongseongArr[jongseongIndex];
            return (ChoseongArr[choseongIndex], JungseongArr[jungseongIndex], jongseong);
        }
        /// <summary>
        /// 입력한 한글 자모를 합쳐서 한글 글자로 만듭니다.
        /// </summary>
        public static char? ToHangeulChar(char choseong, char jungseong, char? jongseong)
        {
            int choseongIndex = Array.IndexOf(ChoseongArr, choseong);
            int jungseongIndex = Array.IndexOf(JungseongArr, jungseong);
            int jongseongIndex = Array.IndexOf(JongseongArr, jongseong);

            if (choseongIndex == -1 || jungseongIndex == -1) return null;
            if (jongseongIndex == -1) jongseongIndex = 0;

            int characterCode = FirstHangeulCode + (choseongIndex * ChoseongStepCount) + (jungseongIndex * JongseongCount) + jongseongIndex;
            return (char)characterCode;
        }
    }
}