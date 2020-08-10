using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Autoccultist.Brain.Config
{
    public class BrainConfig
    {
        public static BrainConfig Load(string filePath)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            var fileContents = File.ReadAllText(filePath);
            return deserializer.Deserialize<BrainConfig>(fileContents);
        }

        // TODO: Verify config is valid

        public List<Goal> Goals { get; set; }
    }
}