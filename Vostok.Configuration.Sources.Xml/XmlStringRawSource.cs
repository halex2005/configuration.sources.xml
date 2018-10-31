using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Xml
{
    internal class XmlStringRawSource : IRawConfigurationSource
    {
        private readonly string xml;
        private volatile bool neverParsed;
        private (ISettingsNode settings, Exception error) currentSettings;
        private XmlDocument doc;

        public XmlStringRawSource(string xml)
        {
            this.xml = xml;
            neverParsed = true;
        }

        private ISettingsNode ParseXml()
        {
            doc = new XmlDocument();
            doc.LoadXml(xml);
            var root = doc.DocumentElement;
            if (root == null) return null;

            var rootNode = ParseElement(root.Name, root);

            return new ObjectNode("root", new Dictionary<string, ISettingsNode>
            {
                [rootNode.Name] = rootNode
            });
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
            
            return new ObjectNode(name, lookup.ToDictionary(elements => elements.Key,
                elements =>
                    elements.Count() == 1
                    ? ParseElement(elements.Key, elements.First())
                    : new ArrayNode(elements.Key, elements.Select((node, index) => ParseElement(index.ToString(), node)).ToList())
                ));
        }

        public IObservable<(ISettingsNode settings, Exception error)> ObserveRaw()
        {
            if (neverParsed)
            {
                neverParsed = false;
                try
                {
                    currentSettings = string.IsNullOrWhiteSpace(xml) ? (null, null) : (ParseXml(), null as Exception);
                }
                catch (Exception e)
                {
                    currentSettings = (null, e);
                }
            }

            return Observable.Return(currentSettings);
        }
    }
}