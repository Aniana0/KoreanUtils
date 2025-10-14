using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KoreanUtils
{
    public static class BaseDataLoader
    {
        private static Assembly? yamlAssembly;
        private static readonly IDeserializer deserializer;

        static BaseDataLoader()
        {
            yamlAssembly = GetYamlAssembly();
            deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        }

        private static Assembly GetYamlAssembly()
        {
            if (yamlAssembly != null) return yamlAssembly;
            var existYamlAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name?.Equals("YamlDotNet", StringComparison.OrdinalIgnoreCase) == true);
            if (existYamlAssembly != null) return existYamlAssembly;

            yamlAssembly = Assembly.Load("YamlDotNet");
            return yamlAssembly!;
        }

        public static T LoadBaseData<T>(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(fileName) ?? throw new FileNotFoundException($"임베드된 YAML 파일을 찾을 수 없습니다: {fileName}");
            using var reader = new StreamReader(stream);
            string yamlText = reader.ReadToEnd();

            return deserializer.Deserialize<T>(yamlText);
        }
    }
}
