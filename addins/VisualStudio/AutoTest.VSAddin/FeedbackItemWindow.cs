using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Messages;

namespace AutoTest.VSAddin
{
    public partial class FeedbackItemWindow : Form
    {
        public bool HasBeenInitialized { get; private set; }
        public event EventHandler<StringArgs> LinkClicked;

        public FeedbackItemWindow()
        {
            InitializeComponent();
            HasBeenInitialized = false;
        }

        public void SetText(string text, string caption)
        {
            Text = caption;
            var parser = new LinkParser(text);
            var links = parser.Parse();
            linkLabel.Text = parser.ParsedText;
            linkLabel.LinkArea = new LinkArea(0, 0);
            foreach (var link in links)
                linkLabel.Links.Add(link.Start, link.Length);
            Height = linkLabel.Height + (linkLabel.Top * 2) + 28;
            Width = linkLabel.Width + (linkLabel.Left * 2) + 8;
        }

        public void BringToFront(Point position)
        {
            Visible = true;
            Show();
            if (!HasBeenInitialized)
                Location = position;
            Activate();
            HasBeenInitialized = true;
        }

        private void FeedbackItemWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (LinkClicked != null)
            {
                var link = linkLabel.Text.Substring(e.Link.Start, e.Link.Length);
                var file = link.Substring(0, link.IndexOf(":line"));
                var lineNumber = getLineNumber(link);
                LinkClicked(this, new StringArgs(file, lineNumber));
            }
        }

        private int getLineNumber(string link)
        {
            var start = link.IndexOf(":line");
            if (start < 0)
                return 0;
            start += ":line".Length;
            return int.Parse(link.Substring(start, link.Length - start));
        }
    }

    public class StringArgs : EventArgs
    {
        public string File { get; private set; }
        public int LineNumber { get; private set; }

        public StringArgs(string file, int lineNumber)
        {
            File = file;
            LineNumber = lineNumber;
        }
    }
}
