namespace KoreanUtils
{
    /// <summary>
    /// 한국어 자모들의 발음 변화와 성질을 태깅한 딕셔너리.
    /// </summary>
    public class JamoPropsYaml
    {
        public Dictionary<string, ConsonantProps> Consonant { get; private set; } = null!;
        public Dictionary<string, VowelProps> Vowel { get; private set; } = null!;
    }

    public class ConsonantProps
    {
        public HashSet<string> Tag { get; init; } = new();
        public ConsonantForms? Forms { get; init; }

    }

    public class VowelProps
    {
        public string? MainVowel { get; init; }
        public string? SemiVowel { get; init; }
        public string? HoldVowel { get; init; }
        public HashSet<string> Tag { get; init; } = new();
    }
    
    public class ConsonantForms
    {
        public string? Tense { get; init; }
        public string? Aspirated { get; init; }
        public string? Liquid { get; init; }
        public string? Nasal { get; init; }
        public string? Default { get; init; }
        public string? Prev { get; init; }
        public string? Next { get; init; }
        public string? Palatalization { get; init; }
    }
}