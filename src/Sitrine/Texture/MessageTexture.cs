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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Sitrine.Texture
{
    public class MessageTexture : Texture, IAnimationTexture
    {
        #region Private Field
        private readonly TextRender render;
        private readonly Queue<object> tokenQueue;

        private int interval = 1;
        private int time = 0;
        private bool endInterval = true;
        private int progressCount = 1;
        private bool updated;
        private PointF charPosition;
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
            this.tokenQueue = new Queue<object>();
        }

        public MessageTexture(TextOptions options, Size size)
            : base(size)
        {
            this.render = new TextRender(options, this.bitmap);
            this.tokenQueue = new Queue<object>();
        }
        #endregion

        #region Public Method
        public void Draw(string text)
        {
            if (!this.Parse(text))
            {
                this.tokenQueue.Clear();
                // TODO: エラー時のメッセージ報告
            }

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
                for (int i = 0; i < this.progressCount && this.tokenQueue.Count > 0; )
                {
                    object token = this.tokenQueue.Dequeue();

                    if (token is char)
                    {
                        char c = (char)token;
                        if (c == '\n')
                        {
                            this.charPosition = new PointF(0.0f, this.charPosition.Y + this.render.Options.LineHeight + 1.0f);
                        }
                        else if (c == ' ')
                        {
                            this.charPosition = new PointF(this.charPosition.X + this.render.Options.Font.Size / 2.0f, this.charPosition.Y);
                        }
                        else if (c == '　')
                        {
                            this.charPosition = new PointF(this.charPosition.X + this.render.Options.Font.Size, this.charPosition.Y);
                        }
                        else
                        {
                            this.updated = true;
                            string ch = c.ToString();
                            SizeF charSize = this.render.DrawChars(c.ToString(), this.charPosition);
                            charSize.Height = 0.0f;
                            this.charPosition = PointF.Add(this.charPosition, charSize);
                        }

                        i++;
                    }
                    else
                    {
                        this.Control((ControlToken)token, ref i);
                    }
                }

                if (this.updated)
                {
                    if (this.TextureUpdate != null)
                        this.TextureUpdate(this, new EventArgs());
                }

                if (this.tokenQueue.Count == 0)
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

            this.charPosition = PointF.Empty;
            this.time = 0;
            this.endInterval = false;
        }

        public override void Render()
        {
            if (this.updated)
            {
                this.updated = false;
                this.render.Flush();
                Texture.Update(this.id, this.bitmap);
            }

            base.Render();
        }
        #endregion

        #region Private Method
        private void Control(ControlToken token, ref int i)
        {
            switch (token.Operate)
            {
                case '\\':
                    this.updated = true;
                    SizeF charSize = this.render.DrawChars("\\", this.charPosition);
                    charSize.Height = 0.0f;
                    this.charPosition = PointF.Add(this.charPosition, charSize);
                    break;

                case 'c':
                    if (this.render.Options.Brushes.Length >= token.Parameter)
                    {
                        // TODO: エラー時のメッセージ報告
                    }
                    else
                        this.render.BrushIndex = token.Parameter;
                    break;

                default:
                    break;
            }
        }

        private bool Parse(string text)
        {
            ControlToken ctoken;
            for (int i = 0, j = text.Length; i < j; i++)
            {
                if (text[i] == '\\')
                {
                    if (!ControlToken.Create(out ctoken, text, ref i))
                        return false;

                    this.tokenQueue.Enqueue(ctoken);
                }
                else
                {
                    this.tokenQueue.Enqueue(text[i]);
                }
            }

            return true;
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
