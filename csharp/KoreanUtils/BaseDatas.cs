using System.Reflection;

namespace KoreanUtils
{
    public static class BaseDatas
    {
        private static readonly ExceptionWordYaml exceptionDict;
        private static readonly JamoPropsYaml jamoProps;
        private static readonly KoreanRuleYaml koreanRules;
        public static Dictionary<string, string> ExceptionWords => exceptionDict.ExceptionWords;
        public static Dictionary<char, ConsonantProps> CProps => jamoProps.Consonant;
        public static Dictionary<char, VowelProps> VProps => jamoProps.Vowel;
        public static HashSet<string> ExceptionLB => koreanRules.ExceptionJongseong11;

        static BaseDatas()
        {
            try
            {
                foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                    Console.WriteLine(name);
                exceptionDict = BaseDataLoader.LoadBaseData<ExceptionWordYaml>("KoreanUtils.exception_words.yaml");
                jamoProps = BaseDataLoader.LoadBaseData<JamoPropsYaml>("KoreanUtils.jamo_properties.yaml");
                koreanRules = BaseDataLoader.LoadBaseData<KoreanRuleYaml>("KoreanUtils.korean_rules.yaml");
                if (exceptionDict == null || jamoProps == null) throw new Exception($"Load Failed : Exception Words is null");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Load Failed] {ex}");
                throw;
            }
        }
    }
}