namespace KoreanUtils
{
    /// <summary>
    /// 한국어 자모들의 발음 변화와 성질을 태깅한 딕셔너리.
    /// </summary>
    public class JamoPropsYaml
    {
        public Dictionary<char, ConsonantProps> Consonant { get; private set; } = null!;
        public Dictionary<char, VowelProps> Vowel { get; private set; } = null!;
    }

    public class ConsonantProps
    {
        public HashSet<string> Tag { get; init; } = new();
        public ConsonantForms? Forms { get; init; }

    }

    public class VowelProps
    {
        public char MainVowel { get; init; }
        public char? SemiVowel { get; init; }
        public char? HoldVowel { get; init; }
        public char EndVowel { get; init; }
        public HashSet<string> Tag { get; init; } = [];
    }
    
    public class ConsonantForms
    {
        public char? Tense { get; init; }
        public char? Aspirated { get; init; }
        public char? Liquid { get; init; }
        public char? Nasal { get; init; }
        public char? Default { get; init; }
        public char? Prev { get; init; }
        public char? Next { get; init; }
        public char? Palatalization { get; init; }
    }
}