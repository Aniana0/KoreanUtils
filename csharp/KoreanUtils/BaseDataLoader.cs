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
        public static readonly IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        public static YamlType LoadBaseYaml<YamlType>(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(fileName) ?? throw new FileNotFoundException($"임베드된 YAML를 찾을 수 없습니다: {fileName}");
            using var reader = new StreamReader(stream);
            var yamlText = reader.ReadToEnd();

            return deserializer.Deserialize<YamlType>(yamlText);
        }
    }
}