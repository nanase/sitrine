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
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Sitrine.Texture
{
    /// <summary>
    /// 文字列をアニメーション表示するためのテクスチャクラスです。
    /// </summary>
    public class MessageTexture : Texture, IAnimationTexture
    {
        #region -- Private Fields --
        private readonly TextRenderer renderer;
        private readonly Queue<object> tokenQueue;

        private int interval = 1;
        private int time = 0;
        private bool endInterval = true;
        private int progressCount = 1;
        private bool updated;
        private PointF charPosition;
        #endregion

        #region -- Public Events --
        /// <summary>
        /// テクスチャが更新されるたびに発生します。
        /// </summary>
        public event EventHandler TextureUpdate;

        /// <summary>
        /// テクスチャの更新が終了したときに発生します。
        /// </summary>
        public event EventHandler TextureEnd;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// メッセージの更新フレーム間隔を取得または設定します。
        /// </summary>
        public int Interval
        {
            get { return this.interval; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                else
                    this.interval = value;
            }
        }

        /// <summary>
        /// メッセージの更新時の文字数を取得または設定します。
        /// </summary>
        public int ProgressCount
        {
            get { return this.progressCount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                else
                    this.progressCount = value;
            }
        }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// テキストレンダラと描画サイズを指定して新しい MessegeTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="renderer">テキストレンダラ。</param>
        /// <param name="size">描画サイズ。</param>
        public MessageTexture(TextRenderer renderer, Size size)
            : base(size)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            this.renderer = renderer;
            this.tokenQueue = new Queue<object>();
        }

        /// <summary>
        /// テキストオプションと描画サイズを指定して新しい MessageTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="options">テキストオプション。</param>
        /// <param name="size">描画サイズ。</param>
        public MessageTexture(TextOptions options, Size size)
            : base(size)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            this.renderer = new TextRenderer(options, this.BaseBitmap);
            this.tokenQueue = new Queue<object>();
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された文字を解析し、描画を予約します。
        /// </summary>
        /// <param name="text">表示される文字列およびコマンド。</param>
        public void Draw(string text)
        {
            if (!this.Parse(text))
            {
                this.tokenQueue.Clear();
                Trace.TraceWarning("Unable to parse string: {0}", (text.Length > 8) ? text.Substring(0, 8) + "..." : text);
            }

            this.TextureEnd = null;
            this.TextureUpdate = null;
            this.Start();
        }

        /// <summary>
        /// 予約された文字列を指定された文字数だけ描画します。
        /// </summary>
        /// <returns>ビットマップに変更が加わりフラッシュの必要があるときは true、それ以外のとき false。</returns>
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
                            this.charPosition = new PointF(0.0f, this.charPosition.Y + this.renderer.Options.LineHeight + 1.0f);
                        }
                        else if (c == ' ')
                        {
                            this.charPosition = new PointF(this.charPosition.X + this.renderer.Options.Font.Size / 2.0f, this.charPosition.Y);
                        }
                        else if (c == '　')
                        {
                            this.charPosition = new PointF(this.charPosition.X + this.renderer.Options.Font.Size, this.charPosition.Y);
                        }
                        else
                        {
                            this.updated = true;
                            string ch = c.ToString();
                            SizeF charSize = this.renderer.DrawChars(c.ToString(), this.charPosition);
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

                return this.updated;
            }
            else
            {
                this.time++;
                return false;
            }
        }

        /// <summary>
        /// ビットマップをクリアし、文字列描画を開始します。
        /// </summary>
        public void Start()
        {
            this.renderer.Clear();
            this.renderer.Flush();

            this.updated = true;
            this.charPosition = PointF.Empty;
            this.time = 0;
            this.endInterval = false;
        }

        /// <summary>
        /// ビットマップに変更を確定させ、画面上にメッセージを表示します。
        /// </summary>
        public override void Render()
        {
            if (this.updated)
            {
                this.updated = false;
                this.renderer.Flush();
                Texture.Update(this.ID, this.BaseBitmap);
            }

            base.Render();
        }
        #endregion

        #region -- Private Methods --
        private void Control(ControlToken token, ref int i)
        {
            switch (token.Operate)
            {
                case '\\':
                    this.updated = true;
                    SizeF charSize = this.renderer.DrawChars("\\", this.charPosition);
                    charSize.Height = 0.0f;
                    this.charPosition = PointF.Add(this.charPosition, charSize);
                    break;

                case 'c':
                    if (this.renderer.Options.Brushes.Length <= token.Parameter)
                        Trace.TraceWarning("Out of brush index: {0}", token.Parameter);
                    else
                        this.renderer.BrushIndex = token.Parameter;

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
            #region -- Private Fields --
            private static readonly Regex tokenizer = new Regex(@"\\(.)(?:\[(\d+)\])?", RegexOptions.Compiled);
            #endregion

            #region -- Public Properties --
            public char Operate { get; private set; }
            public int Parameter { get; private set; }
            #endregion

            #region -- Constructors --
            private ControlToken(char operate, int parameter)
            {
                this.Operate = operate;
                this.Parameter = parameter;
            }
            #endregion

            #region -- Public Static Methods --
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
                index += m.Length - 1;

                return true;
            }
            #endregion
        }
    }
}
