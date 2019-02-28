using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Commons.Testing.Observable;

namespace Vostok.Configuration.Sources.Xml.Tests.Functional
{
    internal class XmlStringSource_Tests
    {
        [Test]
        public void Should_work_correctly()
        {
            var source = new XmlStringSource(TestCase.Xml);
            source.Observe()
                .WaitFirstValue(5.Seconds())
                .Should()
                .BeEquivalentTo((TestCase.SettingsTree, null as Exception));
        }

        [Test]
        public void Should_propagate_changes_to_observers_on_external_push()
        {
            var source = new XmlStringSource("<key></key>");

            source.Push(TestCase.Xml);

            source.Observe()
                .WaitFirstValue(5.Seconds())
                .Should()
                .BeEquivalentTo((TestCase.SettingsTree, null as Exception));
        }
    }
}