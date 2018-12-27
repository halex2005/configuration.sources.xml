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
                .WaitFirstValue(100.Milliseconds())
                .Should()
                .BeEquivalentTo((TestCase.SettingsTree, null as Exception));
        }
    }
}