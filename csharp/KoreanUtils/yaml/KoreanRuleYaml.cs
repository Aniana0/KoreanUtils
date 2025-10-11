namespace KoreanUtils
{
    public class KoreanRuleYaml
    {
        public HashSet<string> ExceptionJongseong11 { get; private set; } = null!;
        public HashSet<char> ForcedJungseong7 { get; private set; } = null!;
        public HashSet<char> VowelsRule15 { get; private set; } = null!;
    }
}