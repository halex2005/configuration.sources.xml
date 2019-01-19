using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Xml
{
    [PublicAPI]
    public static class XmlConfigurationParser
    {
        public static ISettingsNode Parse(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var doc = new XmlDocument();

            doc.LoadXml(content);

            var root = doc.DocumentElement;
            if (root == null)
                return null;

            return new ObjectNode(new List<ISettingsNode> { ParseElement(doc, root.Name, root) });
        }

        private static ISettingsNode ParseElement(XmlDocument doc, string name, XmlElement element)
        {
            if (!element.HasChildNodes && !element.HasAttributes)
                return new ValueNode(name, element.InnerText);

            var childNodes = new List<XmlNode>(element.ChildNodes.Count);

            foreach (XmlNode node in element.ChildNodes)
                childNodes.Add(node);

            foreach (XmlAttribute attribute in element.Attributes)
                if (childNodes.All(n => n.Name != attribute.Name))
                {
                    var elem = doc.CreateElement(attribute.Name);
                    elem.InnerText = attribute.Value;
                    childNodes.Add(elem);
                }

            if (!childNodes.OfType<XmlElement>().Any())
                return new ValueNode(name, element.InnerText);


            var children =
                childNodes
                    .Cast<XmlElement>()
                    .GroupBy(l => l.Name)
                    .Select(
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