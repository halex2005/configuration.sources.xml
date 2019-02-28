using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Commons.Testing.Observable;

namespace Vostok.Configuration.Sources.Xml.Tests.Functional
{
    internal class XmlFileSource_Tests
    {
        [Test]
        public void Should_work_correctly()
        {
            using (var temporaryFile = new TemporaryFile(TestCase.Xml))
            {
                var source = new XmlFileSource(temporaryFile.FileName);
                source.Observe()
                    .WaitFirstValue(5.Seconds())
                    .Should()
                    .BeEquivalentTo((TestCase.SettingsTree, null as Exception));
            }
        }
    }
}