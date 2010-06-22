using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoTest.WinForms
{
    class LinkParser
    {
        public const string TAG_START = "<<Link>>";
        public const string TAG_END = "<</Link>>";

        private string _text;

        public string ParsedText { get { return _text; } }

        public LinkParser(string text)
        {
            _text = text;
        }
        
        public LinkArea[] Parse()
        {
            var links = getLinks();
            removePlaceholders();
            return links.ToArray();
        }

        private List<LinkArea> getLinks()
        {
            var matches = getLinkMatches();
            return getLinkList(matches);
        }

        private List<LinkArea> getLinkList(MatchCollection matches)
        {
            List<LinkArea> links = new List<LinkArea>();
            for (int i = 0; i < matches.Count; i++)
                addLink(i, matches, links);
            return links;
        }

        private MatchCollection getLinkMatches()
        {
            var regExp = new Regex(string.Format("{0}.*?{1}", TAG_START, TAG_END));
            return regExp.Matches(_text);
        }

        private void addLink(int i, MatchCollection matches, List<LinkArea> links)
        {
            links.Add(new LinkArea(getLinkStart(i, matches), getLinkLength(i, matches)));
        }
        private int getLinkStart(int i, MatchCollection matches)
        {
            return matches[i].Index - (i * (TAG_START.Length + TAG_END.Length));
        }

        private int getLinkLength(int i, MatchCollection matches)
        {
            return matches[i].Value.Length - (TAG_START.Length + TAG_END.Length);
        }

        private void removePlaceholders()
        {
            _text = _text.Replace(TAG_START, "");
            _text = _text.Replace(TAG_END, "");
        }
    }
}
