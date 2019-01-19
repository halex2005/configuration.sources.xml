using JetBrains.Annotations;
using Vostok.Configuration.Sources.Constant;

namespace Vostok.Configuration.Sources.Xml
{
    /// <summary>
    /// A source that works by parsing in-memory XML strings.
    /// </summary>
    [PublicAPI]
    public class XmlStringSource : LazyConstantSource
    {
        public XmlStringSource(string xml)
            : base(() => XmlConfigurationParser.Parse(xml))
        {
        }
    }
}