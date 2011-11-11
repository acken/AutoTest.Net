using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTest.UI
{
    public partial class TestDetailsForm : Form
    {
        private Action<string, int> _goToLink;
        private Action<string, string> _goToType;
        private string _currentText = "";
        private int _line = 0;
        private int _lineHeight = 0;
        private List<Link> _links = new List<Link>();

        public TestDetailsForm(Action<string, int> gotToLink, Action<string, string> goToType, int x, int y, string message, string caption, List<Link> links, int maxWidth)
        {
            InitializeComponent();
            SuspendLayout();
            _goToLink = gotToLink;
            _goToType = goToType;
            textBoxFocusHolder.Location = new Point(-1000, -1000);
            textBoxFocusHolder.Select();
            if (x != -1)
                Left = x;
            if (y != -1)
                Top = y;
            SetCaption(caption);
            SetText(message, links, maxWidth);
            ResumeLayout();
        }

        public void SetCaption(string text)
        {
            Text = text;
        }

        public void SetText(string text, List<Link> links, int maxWidth)
        {
            _line = 0;
            _links = links;
            _currentText = text;
            maxWidth = maxWidth - textBoxContent.Left;
            textBoxContent.Text = _currentText;

            var current = getLongestLiine();
            var width = getWidth(current);
            if (width > maxWidth)
                width = maxWidth;
            textBoxContent.Width = width + 6;
            textBoxContent.Height = getHeight(current, width + 13) + 6;
            textBoxContent.SelectionStart = 0;
            textBoxContent.SelectionLength = 0;

            this.Height = 10;
            this.Width = 10;
            textBoxFocusHolder.Select();
            positionArrow();
        }

        private string getLongestLiine()
        {
            var current = "";
            foreach (var str in textBoxContent.Lines)
                if (str.Trim().Length > current.Length)
                    current = str.Trim();
            return current;
        }

        private int getWidth(string current)
        {
            label1.Font = textBoxContent.Font;
            label1.Text = current;
            return label1.Width;
        }

        private int getInternalWidth(string current)
        {
            label1.Font = textBoxContent.Font;
            label1.Text = current;
            return label1.Width;
        }

        private int getHeight(string current, int width)
        {
            if (textBoxContent.TextLength == 0)
                return 0;

            var gfx = textBoxContent.CreateGraphics();
            var size = gfx.MeasureString(current, textBoxContent.Font);
            _lineHeight = ((int)gfx.MeasureString(current, textBoxContent.Font).Height);
            gfx.Dispose();

            var pt = textBoxContent.GetPositionFromCharIndex(textBoxContent.TextLength - 1);
            return pt.Y + _lineHeight;
        }

        private void textBoxContent_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Shift && e.KeyCode != Keys.Enter && e.KeyCode != Keys.H) || e.Control || e.Alt)
                return;

            if ( e.KeyCode == Keys.J || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (_links.Count == 0 || _line == (_links.Count - 1))
                    return;
                _line++;
                positionArrow();
            }
            else if (e.KeyCode == Keys.K || e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (_line == 0)
                    return;
                _line--;
                positionArrow();
            }
            else if (e.Shift && e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (_links.Count != 0)
                {
                    var link = _links[_line];
                    goTo(link);
                    BringToFront();
                }
            }
            else if (!e.Shift && e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (_links.Count != 0)
                {
                    var link = _links[_line];
                    goTo(link);
                    Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Visible = false;
            }
            else if (e.Shift && e.KeyCode == Keys.H)
            {
                AllowTransparency = true;
                e.Handled = true;
                e.SuppressKeyPress = true;
                Opacity = 0.2;
            }
        }

        private void goTo(Link link)
        {
            if (link.File == null)
                _goToType(link.Assembly, link.Type);
            else
                _goToLink(link.File, link.Line);
        }

        private void textBoxContent_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.H)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Opacity = 1;
            }
        }

        private void positionArrow()
        {
            pictureBox1.Visible = _links.Count > 0;
            pictureBoxLine.Visible = _links.Count > 0;
            if (_links.Count != 0)
            {
                var pt = textBoxContent.GetPositionFromCharIndex(_links[_line].IndexStart);
                pictureBox1.Top = textBoxContent.Top + 3 + pt.Y;
                pictureBoxLine.Top = pictureBox1.Top;
                var ptEnd = textBoxContent.GetPositionFromCharIndex(_links[_line].IndexEnd);
                if (ptEnd.Y > pt.Y)
                    pictureBoxLine.Height = (ptEnd.Y - pt.Y) + pictureBox1.Height;
                else
                    pictureBoxLine.Height = pictureBox1.Height;
            }
        }

        private void TestDetailsForm2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void textBoxFocusHolder_KeyDown(object sender, KeyEventArgs e)
        {
            textBoxContent_KeyDown(sender, e);
        }

        private void textBoxFocusHolder_KeyUp(object sender, KeyEventArgs e)
        {
            textBoxContent_KeyUp(sender, e);
        }

        private void textBoxContent_MouseDown(object sender, MouseEventArgs e)
        {
            var index = textBoxContent.GetCharIndexFromPosition(new Point(e.X, e.Y));
            var linkIndex = 0;
            foreach (var link in _links)
            {
                if (link.IndexStart <= index && link.IndexEnd >= index)
                {
                    _line = linkIndex;
                    positionArrow();
                    break;
                }
                linkIndex++;
            }
        }

        private void textBoxContent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = textBoxContent.GetCharIndexFromPosition(new Point(e.X, e.Y));
            var linkIndex = 0;
            foreach (var link in _links)
            {
                if (link.IndexStart <= index && link.IndexEnd >= index)
                {
                    _line = linkIndex;
                    positionArrow();
                    goTo(link);
                    Visible = false;
                    break;
                }
                linkIndex++;
            }
        }
    }

    public class Link
    {
        public int IndexStart { get; private set; }
        public int IndexEnd { get; private set; }
        public string File { get; private set; }
        public int Line { get; private set; }

        public string Assembly { get; private set; }
        public string Type { get; private set; }

        public Link(int start, int end, string file, int line)
        {
            IndexStart = start;
            IndexEnd = end;
            File = file;
            Line = line;
        }

        public Link(int start, int end, string assembly, string type)
        {
            IndexStart = start;
            IndexEnd = end;
            Assembly = assembly;
            Type = type;
            File = null;
            Line = 0;
        }
    }
}
