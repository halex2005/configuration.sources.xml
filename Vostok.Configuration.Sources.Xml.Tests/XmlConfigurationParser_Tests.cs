using System;
using System.Linq;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;

// ReSharper disable PossibleNullReferenceException

namespace Vostok.Configuration.Sources.Xml.Tests
{
    [TestFixture]
    public class XmlConfigurationParser_Tests
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

            settings.Should().BeOfType<ValueNode>().Which.Value.Should().Be(value);
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

            settings.Name.Should().Be("Dictionary");
            settings["Key1"].Value.Should().Be("value1");
            settings["Key2"].Value.Should().Be("value2");
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

            settings.Name.Should().Be("ArrayParent");
            settings["Array"].Children.Select(child => child.Value).Should().Equal("value1", "value2");
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

            settings.Name.Should().Be("object");
            settings["item"].Children.Count().Should().Be(2);
            settings["item"].Children.First()["subitem1"].Value.Should().Be("value1");
            settings["item"].Children.Last()["subitem2"].Value.Should().Be("value2");
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

            settings.Name.Should().Be("object");
            settings["item"].Children.Count().Should().Be(2);
            settings["item"].Children.First()["subitem"].Children.Select(c => c.Value).Should().Equal("value1", "value2");
            settings["item"].Children.Last()["subitem"].Children.Select(c => c.Value).Should().Equal("value3", "value4");
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

            settings.Name.Should().Be("Mixed");
            settings["SingleItem"].Value.Should().Be("SingleValue");
            settings["DictItem"]["DictKey"].Value.Should().Be("DictValue");
            settings["ArrayItem"].Children.Select(child => child.Value).Should().Equal("ArrayValue1", "ArrayValue2");
        }

        [Test]
        public void Should_parse_Object_from_attributes()
        {
            var settings = XmlConfigurationParser.Parse("<object key1='val1' key2='val2' />");

            settings.Name.Should().Be("object");
            settings["key1"].Value.Should().Be("val1");
            settings["key2"].Value.Should().Be("val2");
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

            settings.Name.Should().Be("object");
            settings["item"].Children.Select(child => child.Value).Should().Equal("value1", "value2");
            settings["attr"].Value.Should().Be("test");
        }

        [Test]
        public void Should_ignore_key_case()
        {
            var settings = XmlConfigurationParser.Parse("<root><value>string</value></root>");

            settings["VALUE"].Value.Should().Be("string");
        }

         [Test]
         public void Should_throw_XmlException_on_wrong_xml_format()
         {
             const string value = "wrong file format";
             new Action(() => { XmlConfigurationParser.Parse(value); }).Should().Throw<XmlException>();
         }

         [Test]
         public void Should_parse_XmlDocument_with_overriden_root_name()
         {
             var settings = XmlConfigurationParser.Parse("<object key1='val1' key2='val2' />", "overriden");

             settings.Name.Should().Be("overriden");
             settings["key1"].Value.Should().Be("val1");
             settings["key2"].Value.Should().Be("val2");
         }
    }
}