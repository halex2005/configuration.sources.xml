using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Xml.Tests.Functional
{
    internal static class TestCase
    {
        public static readonly string Xml = "<root><key> <a>1</a> <a>2</a> <a>3</a> </key></root>";

        public static readonly ISettingsNode SettingsTree = new ObjectNode("root",
            new[]
            {
                new ObjectNode(
                    "key",
                    new[]
                    {
                        new ArrayNode("a", new[]
                        {
                            new ValueNode("0", "1"),
                            new ValueNode("1", "2"),
                            new ValueNode("2", "3")
                        })
                    })
            });
    }
}