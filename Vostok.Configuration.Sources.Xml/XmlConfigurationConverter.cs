using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Xml
{
    internal class XmlConfigurationConverter : IConfigurationConverter<string>
    {
        private XmlDocument doc;

        public ISettingsNode Convert(string configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration))
                return null;
            doc = new XmlDocument();
            doc.LoadXml(configuration);
            var root = doc.DocumentElement;
            if (root == null) return null;

            var rootNode = ParseElement(root.Name, root);

            return new ObjectNode("root", new List<ISettingsNode> {rootNode});
        }

        private ISettingsNode ParseElement(string name, XmlElement element)
        {
            if (!element.HasChildNodes && !element.HasAttributes)
                return new ValueNode(name, element.InnerText);

            var nodeList = new List<XmlNode>(element.ChildNodes.Count);
            foreach (XmlNode node in element.ChildNodes)
                nodeList.Add(node);
            foreach (XmlAttribute attribute in element.Attributes)
                if (nodeList.All(n => n.Name != attribute.Name))
                {
                    var elem = doc.CreateElement(attribute.Name);
                    elem.InnerText = attribute.Value;
                    nodeList.Add(elem);
                }

            if (!nodeList.OfType<XmlElement>().Any())
                return new ValueNode(name, element.InnerText);

            var lookup = nodeList.Cast<XmlElement>().ToLookup(l => l.Name);

            var children = lookup.Select(
                    elements =>
                        elements.Count() == 1
                            ? ParseElement(elements.Key, elements.First())
                            : new ArrayNode(elements.Key, elements.Select((node, index) => ParseElement(index.ToString(), node)).ToList())
                )
                .ToList();
            
            return new ObjectNode(name, children);
        }
    }
}