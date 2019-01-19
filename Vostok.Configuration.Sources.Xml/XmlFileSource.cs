using JetBrains.Annotations;
using Vostok.Configuration.Sources.File;

namespace Vostok.Configuration.Sources.Xml
{
    /// <summary>
    /// A source that parses settings from local files in XML format.
    /// </summary>
    [PublicAPI]
    public class XmlFileSource : FileSource
    {
        public XmlFileSource([NotNull] string filePath)
            : this(new FileSourceSettings(filePath))
        {
        }
        
        public XmlFileSource([NotNull] FileSourceSettings settings)
            : base(settings, XmlConfigurationParser.Parse)
        {
        }
    }
}