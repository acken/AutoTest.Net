using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.WinForms.Test
{
    [TestFixture]
    public class LinkParserTest
    {
        [Test]
        public void Should_replace_link_tags()
        {
            var parser = new LinkParser("some text <<Link>>some more text<</Link>> and yet <<Link>>some more<</Link>>");
            var links = parser.Parse();
            parser.ParsedText.ShouldEqual("some text some more text and yet some more");
			links.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_return_links()
        {
            var parser = new LinkParser("some text <<Link>>some more text<</Link>> and yet <<Link>>some more<</Link>>");
            var links = parser.Parse();
            
            links.Length.ShouldEqual(2);
            // some more text
            links[0].Start.ShouldEqual(10);
            links[0].Length.ShouldEqual(14);
            // some more
            links[1].Start.ShouldEqual(33);
            links[1].Length.ShouldEqual(9);
        }
    }
}
