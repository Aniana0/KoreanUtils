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
        public static YamlType LoadBaseData<YamlType>(string yamlName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(yamlName) ?? throw new FileNotFoundException($"Load Base Data File Failed : {yamlName}");
            using var reader = new StreamReader(stream);
            var yamlText = reader.ReadToEnd();

            return deserializer.Deserialize<YamlType>(yamlText);
        }
    }
}