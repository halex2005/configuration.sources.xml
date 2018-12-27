using System;
using System.Linq;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace Vostok.Configuration.Sources.Xml.Tests
{
    [TestFixture]
    public class XmlConfigurationConverter_Tests
    {
        [TestCase(null, TestName = "when string is null")]
        [TestCase(" ", TestName = "when string is whitespace")]
        public void Should_return_null(string xml)
        {
            XmlConfigurationParser.Parse(xml).Should().BeNull();
        }

        [TestCase("")]
        [TestCase("string")]
        public void Should_parse_single_value(string value)
        {
            var settings = XmlConfigurationParser.Parse($"<StringValue>{value}</StringValue>");
            settings["StringValue"].Value.Should().Be(value);
        }

        [Test]
        public void Should_parse_Object_from_subelements()
        {
            const string value = @"
<Dictionary>
    <Key1>value1</Key1>
    <Key2>value2</Key2>
</Dictionary>";
            
            var settings = XmlConfigurationParser.Parse(value);

            settings["Dictionary"]["Key1"].Value.Should().Be("value1");
            settings["Dictionary"]["Key2"].Value.Should().Be("value2");
        }

        [Test]
        public void Should_parse_Array_from_elements_with_same_keys()
        {
            const string value = @"
<ArrayParent>
    <Array>value1</Array>
    <Array>value2</Array>
</ArrayParent>";
            
            var settings = XmlConfigurationParser.Parse(value);
            
            settings["ArrayParent"]["Array"].Children.Select(child => child.Value).Should().Equal("value1", "value2");
        }

        [Test]
        public void Should_parse_ArrayOfObjects_value()
        {
            const string value = @"
<object>
    <item>
        <subitem1>value1</subitem1>
    </item>
    <item>
        <subitem2>value2</subitem2>
    </item>
</object>";

            var settings = XmlConfigurationParser.Parse(value);
            settings["object"]["item"].Children.Count().Should().Be(2);
            settings["object"]["item"].Children.First()["subitem1"].Value.Should().Be("value1");
            settings["object"]["item"].Children.Last()["subitem2"].Value.Should().Be("value2");
        }
        
        [Test]
        public void Should_parse_ArrayOfArrays_value()
        {
            const string value = @"
<object>
    <item>
        <subitem>value1</subitem>
        <subitem>value2</subitem>
    </item>
    <item>
        <subitem>value3</subitem>
        <subitem>value4</subitem>
    </item>
</object>";

            var settings = XmlConfigurationParser.Parse(value);
            settings["object"]["item"].Children.Count().Should().Be(2);
            settings["object"]["item"].Children.First()["subitem"].Children.Select(c => c.Value).Should().Equal("value1", "value2");
            settings["object"]["item"].Children.Last()["subitem"].Children.Select(c => c.Value).Should().Equal("value3", "value4");
        }

        [Test]
        public void Should_parse_Object_with_children_of_different_types()
        {
            const string value = @"
<Mixed>
    <SingleItem>SingleValue</SingleItem>
    <DictItem>
        <DictKey>DictValue</DictKey>
    </DictItem>
    <ArrayItem>ArrayValue1</ArrayItem>
    <ArrayItem>ArrayValue2</ArrayItem>
</Mixed>";
            
            var settings = XmlConfigurationParser.Parse(value);

            settings["Mixed"]["SingleItem"].Value.Should().Be("SingleValue");
            settings["Mixed"]["DictItem"]["DictKey"].Value.Should().Be("DictValue");
            settings["Mixed"]["ArrayItem"].Children.Select(child => child.Value).Should().Equal("ArrayValue1", "ArrayValue2");
        }
        
        [Test]
        public void Should_parse_Object_from_attributes()
        {
            var settings = XmlConfigurationParser.Parse("<object key1='val1' key2='val2' />");
         
            settings["Object"]["key1"].Value.Should().Be("val1");
            settings["Object"]["key2"].Value.Should().Be("val2");
        }

        [Test]
        public void Should_ignore_attributes_presented_in_subelements_and_add_not_presented_in_subelements()
        {
            const string value = @"
<object item='value0' attr='test'>
    <item>value1</item>
    <item>value2</item>
</object>";

            var settings = XmlConfigurationParser.Parse(value);
            settings["Object"]["item"].Children.Select(child => child.Value).Should().Equal("value1", "value2");
            settings["Object"]["attr"].Value.Should().Be("test");
        }
        
        [Test]
        public void Should_ignore_key_case()
        {
            var settings = XmlConfigurationParser.Parse("<value>string</value>");
            settings["VALUE"].Value.Should().Be("string");
        }

         [Test]
         public void Should_throw_XmlException_on_wrong_xml_format()
         {
             const string value = "wrong file format";
             new Action(() => { XmlConfigurationParser.Parse(value); }).Should().Throw<XmlException>();
         }
    }
}