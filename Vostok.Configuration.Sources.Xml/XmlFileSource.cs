using System;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.File;

namespace Vostok.Configuration.Sources.Xml
{
    /// <inheritdoc />
    /// <summary>
    /// Xml converter to <see cref="ISettingsNode"/> tree from file
    /// </summary>
    public class XmlFileSource : BaseFileSource
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="XmlFileSource"/> instance using given parameter <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">File name with settings</param>
        /// <param name="settings">File parsing settings</param>
        public XmlFileSource([NotNull] string filePath)
            : this(new FileSourceSettings{FilePath = filePath})
        {
        }
        
        public XmlFileSource(FileSourceSettings settings)
            : base(settings, ParseSettings)
        {
        }

        private static ISettingsNode ParseSettings(string str) => new XmlConfigurationConverter().Convert(str);
    }
}