/* Sitrine - 2D Game Library with OpenTK */

/* LICENSE - The MIT License (MIT)

Copyright (c) 2013 Tomona Nanase

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Sitrine.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Sitrine.Texture
{
    public class MessageTexture : Texture, IAnimationTexture
    {
        #region Private Field
        private readonly TextRender render;

        private int interval = 1;
        private int time = 0;
        private int index = 0;
        private bool endInterval = true;
        private int progressCount = 1;
        private string text;

        private PointF forePosition;
        private PointF shadowPosition;
        #endregion

        #region Public Event
        public event EventHandler TextureUpdate;
        public event EventHandler TextureEnd;
        #endregion

        #region Public Property
        public int Interval
        {
            get { return this.interval; }
            set { if (value <= 0) throw new ArgumentOutOfRangeException(); else this.interval = value; }
        }

        public int ProgressCount
        {
            get { return this.progressCount; }
            set { if (value <= 0) throw new ArgumentOutOfRangeException(); else this.progressCount = value; }
        }
        #endregion

        #region Constructor
        public MessageTexture(TextRender render, Size size)
            : base(size)
        {
            this.render = render;
        }
        #endregion

        #region Public Method
        public new void Draw(string text)
        {
            this.text = text;
            this.TextureEnd = null;
            this.TextureUpdate = null;
            this.Start();
        }

        public bool Update()
        {
            if (this.endInterval)
                return false;

            if (this.time >= this.interval)
            {
                bool updated = false;
                for (int j = Math.Min(this.index + this.progressCount, this.text.Length); this.index < j; this.index++)
                {
                    char c = text[this.index];
                    if (c == '\n')
                    {
                        this.forePosition = new Point((int)this.forePoint.X, this.forePosition.Y + (this.options.LineHeight + 1));
                        this.shadowPosition = new Point((int)this.shadowPoint.X, this.shadowPosition.Y + (this.options.LineHeight + 1));
                    }
                    else if (c == ' ')
                    {
                        this.forePosition = new Point(this.forePosition.X + this.fontSize / 2, this.forePosition.Y);
                        this.shadowPosition = new Point(this.shadowPosition.X + this.fontSize / 2, this.shadowPosition.Y);
                    }
                    else if (c == '　')
                    {
                        this.forePosition = new Point(this.forePosition.X + this.fontSize, this.forePosition.Y);
                        this.shadowPosition = new Point(this.shadowPosition.X + this.fontSize, this.shadowPosition.Y);
                    }
                    else
                    {
                        updated = true;
                        string ch = c.ToString();
                        this.g.DrawString(ch, this.options.Font, this.shadowBrush, this.shadowPosition, this.options.Format);
                        this.g.DrawString(ch, this.options.Font, this.foreBrush, this.forePosition, this.options.Format);

                        Size charSize = this.g.MeasureString(ch, this.options.Font, this.forePosition, this.options.Format).ToSize();

                        this.forePosition = Point.Add(this.forePosition, new Size(charSize.Width, 0));
                        this.shadowPosition = Point.Add(this.shadowPosition, new Size(charSize.Width, 0));
                    }
                }

                if (updated)
                {
                    this.g.Flush();
                    Texture.Update(this.id, this.bitmap);

                    if (this.TextureUpdate != null)
                        this.TextureUpdate(this, new EventArgs());
                }

                if (this.index >= this.text.Length)
                {
                    this.endInterval = true;
                    if (this.TextureEnd != null)
                        this.TextureEnd(this, new EventArgs());
                }

                this.time = 0;

                return true;
            }
            else
            {
                this.time++;
                return false;
            }
        }

        public void Start()
        {
            this.render.Clear();

            this.forePosition = PointF.Empty;
            this.shadowPosition = PointF.Empty;

            this.index = 0;
            this.time = 0;
            this.endInterval = false;
        }
        #endregion

        #region Private Method
        private void Parse(string text)
        {
            throw new NotImplementedException();
        }
        #endregion

        class ControlToken
        {
            #region Private Field
            private static readonly Regex tokenizer = new Regex(@"\\(.)(\[\d+\])?", RegexOptions.Compiled);
            #endregion

            #region Public Property
            public char Operate { get; private set; }
            public int Parameter { get; private set; }
            #endregion

            #region Constructor
            private ControlToken(char operate, int parameter)
            {
                this.Operate = operate;
                this.Parameter = parameter;
            }
            #endregion

            #region Public Method
            public static bool Create(out ControlToken token, string source, ref int index)
            {
                char operate;
                int parameter = 0;

                token = null;

                Match m = ControlToken.tokenizer.Match(source, index);

                if (!m.Success || 
                    m.Groups[2].Success && !int.TryParse(m.Groups[2].Value, out parameter))
                    return false;

                operate = m.Groups[1].Value[0];

                token = new ControlToken(operate, parameter);
                index += m.Length;

                return true;
            }
            #endregion
        }
    }
}
