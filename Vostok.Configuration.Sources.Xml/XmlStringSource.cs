using JetBrains.Annotations;
using Vostok.Configuration.Sources.Manual;

namespace Vostok.Configuration.Sources.Xml
{
    /// <summary>
    /// A source that works by parsing in-memory XML strings.
    /// </summary>
    [PublicAPI]
    public class XmlStringSource : ManualFeedSource<string>
    {
        public XmlStringSource(string xml)
            : base(XmlConfigurationParser.Parse)
        {
            Push(xml);
        }
    }
}