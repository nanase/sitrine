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

using System;
using System.Drawing;

namespace Sitrine.Utils
{
    /// <summary>
    /// テキストの描画に用いられるオプションを格納します。
    /// </summary>
    public class TextOptions
    {
        #region -- Private Fields --
        private float lineHeight;
        private Brush[] brushes;
        private Font font;
        private int shadowIndex = 0;
        private StringFormat format;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// 行の高さを取得または設定します。
        /// </summary>
        public float LineHeight
        {
            get
            {
                return this.lineHeight;
            }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException("value");

                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("value");

                this.lineHeight = value;
            }
        }

        /// <summary>
        /// 描画時に文字に塗りつぶされるブラシオブジェクトの配列を取得または設定します。
        /// </summary>
        public Brush[] Brushes
        {
            get
            {
                return this.brushes;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length == 0)
                    throw new ArgumentException("少なくとも 1 つのブラシを指定する必要があります。");

                this.brushes = value;
            }
        }

        /// <summary>
        /// アンチエイリアスを使用するかの真偽値を取得または設定します。
        /// </summary>
        public bool Antialiasing { get; set; }

        /// <summary>
        /// 影の描画を行うかの真偽値を取得または設定します。
        /// </summary>
        public bool DrawShadow { get; set; }

        /// <summary>
        /// 影の描画色のインデクスを取得または設定します。
        /// </summary>
        public int ShadowIndex
        {
            get
            {
                return this.shadowIndex;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                this.shadowIndex = value;
            }
        }

        /// <summary>
        /// フォントを取得または設定します。
        /// </summary>
        public Font Font
        {
            get
            {
                return this.font;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.font = value;
            }
        }

        /// <summary>
        /// 文字列の描画時に用いられる StringFormat オブジェクトを取得または設定します。
        /// </summary>
        public StringFormat Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.format = value;
            }
        }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// フォントと行の高さを指定して新しい TextOptions クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="font">描画に用いるフォント。</param>
        /// <param name="lineHeight">行の高さ。</param>
        public TextOptions(Font font, int lineHeight)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if (lineHeight < 0)
                throw new ArgumentOutOfRangeException("lineHeight");

            this.font = font;
            this.LineHeight = lineHeight;
            this.brushes = new Brush[0];
            this.format = new StringFormat(StringFormat.GenericTypographic)
            {
                 FormatFlags = StringFormatFlags.NoWrap
            };
        }

        /// <summary>
        /// フォントファミリを指定して新しい TextOptions クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="fontFamily">フォントファミリ。</param>
        /// <param name="fontSize">フォントのサイズ。単位はピクセルです。</param>
        /// <param name="lineHeight">行の高さ。</param>
        /// <param name="style">フォントのスタイル。</param>
        public TextOptions(FontFamily fontFamily, int fontSize, int lineHeight, FontStyle style = FontStyle.Regular)
        {
            if (fontFamily == null)
                throw new ArgumentNullException("fontFamily");

            if (fontSize < 0)
                throw new ArgumentOutOfRangeException("fontSize");

            if (lineHeight < 0)
                throw new ArgumentOutOfRangeException("lineHeight");

            this.font = new Font(fontFamily, fontSize, style, GraphicsUnit.Pixel);
            this.LineHeight = lineHeight;
            this.brushes = new Brush[0];
            this.format = new StringFormat(StringFormat.GenericTypographic)
            {
                FormatFlags = StringFormatFlags.NoWrap
            };
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// Color 構造体の配列を使って Brushes プロパティを設定します。
        /// </summary>
        /// <param name="colors"></param>
        public void SetSolidBrushes(params Color[] colors)
        {
            if (colors == null)
                throw new ArgumentNullException("colors");

            if (colors.Length == 0)
                throw new ArgumentException("少なくとも 1 つの色を指定する必要があります。");

            this.brushes = new Brush[colors.Length];

            for (int i = 0, j = colors.Length; i < j; i++)
                this.brushes[i] = new SolidBrush(colors[i]);
        }
        #endregion
    }
}
