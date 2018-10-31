using System;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Xml
{
    /// <inheritdoc />
    /// <summary>
    /// Xml converter to <see cref="ISettingsNode"/> tree from string
    /// </summary>
    public class XmlStringSource : ConfigurationSourceAdapter
    {
        /// <summary>
        /// <para>Creates a <see cref="XmlStringSource"/> instance using given string in <paramref name="xml"/> parameter</para>
        /// <para>Parsing is here.</para>
        /// </summary>
        /// <param name="xml">xml data in string</param>
        /// <exception cref="Exception">Xml has wrong format</exception>
        public XmlStringSource(string xml)
            : base(new XmlStringRawSource(xml))
        {
        }
    }
}