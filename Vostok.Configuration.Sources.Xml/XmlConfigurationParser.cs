using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Xml
{
    internal static class XmlConfigurationParser
    {
        public static ISettingsNode Parse(string configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration))
                return null;
            var doc = new XmlDocument();
            doc.LoadXml(configuration);
            var root = doc.DocumentElement;
            if (root == null) return null;

            var rootNode = ParseElement(doc, root.Name, root);

            return new ObjectNode(new List<ISettingsNode> {rootNode});
        }

        private static ISettingsNode ParseElement(XmlDocument doc, string name, XmlElement element)
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
                            ? ParseElement(doc, elements.Key, elements.First())
                            : new ArrayNode(elements.Key, elements.Select((node, index) => ParseElement(doc, index.ToString(), node)).ToList())
                )
                .ToList();
            
            return new ObjectNode(name, children);
        }
    }
}