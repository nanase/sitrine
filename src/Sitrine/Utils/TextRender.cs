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
using System.Drawing.Text;

namespace Sitrine.Utils
{
    /// <summary>
    /// ビットマップに文字列を描画するための機能を提供します。
    /// </summary>
    public class TextRender : IDisposable
    {
        #region -- Private Fields --
        private static readonly PointF foreOffset;
        private static readonly PointF shadowOffset;

        private readonly Graphics graphics;
        private readonly Bitmap bitmap;
        private readonly TextOptions options;

        private int brushIndex = 0;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// 文字列描画時のブラシのインデクスを取得または設定します。
        /// </summary>
        public int BrushIndex
        {
            get
            {
                return this.brushIndex;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                this.brushIndex = value;
            }
        }

        /// <summary>
        /// 描画に用いるテキストオプションを取得します。
        /// </summary>
        public TextOptions Options { get { return this.options; } }
        #endregion

        #region -- Constructors --
        static TextRender()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                TextRender.foreOffset = new PointF(1, -1);
                TextRender.shadowOffset = new PointF(2, 0);
            }
            else
            {
                TextRender.foreOffset = new PointF(0, -2);
                TextRender.shadowOffset = new PointF(1, -1);
            }
        }

        /// <summary>
        /// テキストオプションと描画先のビットマップを指定して新しい TextRender クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">使用されるテキストオプション。</param>
        /// <param name="baseBitmap">描画先のビットマップ。</param>
        public TextRender(TextOptions options, Bitmap baseBitmap)
        {
            if (baseBitmap == null)
                throw new ArgumentNullException();

            this.options = options;
            this.bitmap = baseBitmap;
            this.graphics = Graphics.FromImage(baseBitmap);
        }

        /// <summary>
        /// テキストオプションと描画先のテクスチャを指定して新しい TextRender クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">使用されるテキストオプション。</param>
        /// <param name="texture">描画先のテクスチャ。</param>
        public TextRender(TextOptions options, Texture.Texture texture)
        {
            if (texture == null)
                throw new ArgumentNullException();

            this.options = options;
            this.bitmap = texture.BaseBitmap;
            this.graphics = Graphics.FromImage(texture.BaseBitmap);
        }
        #endregion

        #region -- Public Methods --
        #region DrawString(string text, float x, float y)
        /// <summary>
        /// 文字列を描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        public void DrawString(string text)
        {
            this.DrawString(text, this.brushIndex, 0.0f, 0.0f);
        }

        /// <summary>
        /// 文字列を描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="location">描画される原点位置。</param>
        public void DrawString(string text, PointF location)
        {
            this.DrawString(text, this.brushIndex, location.X, location.Y);
        }

        /// <summary>
        /// 文字列を描画します。 
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="x">描画される原点の X 座標。</param>
        /// <param name="y">描画される原点の Y 座標。</param>
        public void DrawString(string text, float x, float y)
        {
            this.DrawString(text, this.brushIndex, x, y);
        }
        #endregion

        #region DrawString(string text, int brushIndex, float x, float y)
        /// <summary>
        /// 文字列を指定されたブラシで描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="brushIndex">ブラシのインデクス。</param>
        public void DrawString(string text, int brushIndex)
        {
            this.DrawString(text, brushIndex, 0.0f, 0.0f);
        }

        /// <summary>
        /// 文字列を指定されたブラシで描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="brushIndex">ブラシのインデクス。</param>
        /// <param name="location">描画される原点位置。</param>
        public void DrawString(string text, int brushIndex, PointF location)
        {
            this.DrawString(text, brushIndex, location.X, location.Y);
        }

        /// <summary>
        /// 文字列を指定されたブラシで描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="brushIndex">ブラシのインデクス。</param>
        /// <param name="x">描画される原点の X 座標。</param>
        /// <param name="y">描画される原点の Y 座標。</param>
        public void DrawString(string text, int brushIndex, float x, float y)
        {
            this.graphics.TextRenderingHint = (this.options.Antialiasing) ? TextRenderingHint.AntiAlias : TextRenderingHint.SingleBitPerPixel;

            int i = 0;
            bool shadow = (this.options.DrawShadow && this.options.ShadowIndex > 0 && this.options.ShadowIndex < this.options.Brushes.Length);
            bool fore = (brushIndex >= 0 && brushIndex < this.options.Brushes.Length);

            foreach (var line in text.Split('\n'))
            {
                float y_offset = i * (this.options.LineHeight + 1.0f) + y;

                if (shadow)
                    this.graphics.DrawString(line, this.options.Font, this.options.Brushes[this.options.ShadowIndex], TextRender.shadowOffset.X + x, TextRender.shadowOffset.Y + y_offset, this.options.Format);

                if (fore)
                    this.graphics.DrawString(line, this.options.Font, this.options.Brushes[brushIndex], TextRender.foreOffset.X + x, TextRender.foreOffset.Y + y_offset, this.options.Format);

                i++;
            }
        }
        #endregion

        #region DrawChars(string text, float x, float y)
        /// <summary>
        /// 文字列を描画し、その描画サイズを返します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <returns>描画された矩形領域のサイズを返します。</returns>
        public SizeF DrawChars(string text)
        {
            return this.DrawChars(text, 0.0f, 0.0f);
        }

        /// <summary>
        /// 文字列を描画し、その描画サイズを返します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="location">描画される原点位置。</param>
        /// <returns>描画された矩形領域のサイズを返します。</returns>
        public SizeF DrawChars(string text, PointF location)
        {
            return this.DrawChars(text, location.X, location.Y);
        }

        /// <summary>
        /// 文字列を描画し、その描画サイズを返します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="x">描画される原点の X 座標。</param>
        /// <param name="y">描画される原点の Y 座標。</param>
        /// <returns>描画された矩形領域のサイズを返します。</returns>
        public SizeF DrawChars(string text, float x, float y)
        {
            return this.DrawChars(text, this.brushIndex, x, y);
        }
        #endregion

        #region DrawChars(string text, int brushIndex, float x, float y)
        /// <summary>
        /// 文字列を描画し、その描画サイズを返します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="brushIndex">ブラシのインデクス。</param>
        /// <returns>描画された矩形領域のサイズを返します。</returns>
        public SizeF DrawChars(string text, int brushIndex)
        {
            return this.DrawChars(text, brushIndex, 0.0f, 0.0f);
        }

        /// <summary>
        /// 文字列を描画し、その描画サイズを返します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="brushIndex">ブラシのインデクス。</param>
        /// <param name="location">描画される原点位置。</param>
        /// <returns>描画された矩形領域のサイズを返します。</returns>
        public SizeF DrawChars(string text, int brushIndex, PointF location)
        {
            return this.DrawChars(text, brushIndex, location.X, location.Y);
        }

        /// <summary>
        /// 文字列を描画し、その描画サイズを返します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="brushIndex">ブラシのインデクス。</param>
        /// <param name="x">描画される原点の X 座標。</param>
        /// <param name="y">描画される原点の Y 座標。</param>
        /// <returns>描画された矩形領域のサイズを返します。</returns>
        public SizeF DrawChars(string text, int brushIndex, float x, float y)
        {
            this.graphics.TextRenderingHint = (this.options.Antialiasing) ? TextRenderingHint.AntiAlias : TextRenderingHint.SingleBitPerPixel;

            if (this.options.DrawShadow && this.options.ShadowIndex > 0 && this.options.ShadowIndex < this.options.Brushes.Length)
                this.graphics.DrawString(text, this.options.Font, this.options.Brushes[this.options.ShadowIndex], TextRender.shadowOffset.X + x, TextRender.shadowOffset.Y + y, this.options.Format);

            if (brushIndex >= 0 && brushIndex < this.options.Brushes.Length)
                this.graphics.DrawString(text, this.options.Font, this.options.Brushes[brushIndex], TextRender.foreOffset.X + x, TextRender.foreOffset.Y + y, this.options.Format);

            return this.graphics.MeasureString(text, this.options.Font, PointF.Empty, this.options.Format);
        }
        #endregion

        /// <summary>
        /// ビットマップをクリアします。
        /// </summary>
        public void Clear()
        {
            this.graphics.Clear(Color.Transparent);
        }

        /// <summary>
        /// ビットマップへの変更を確定します。
        /// </summary>
        public void Flush()
        {
            this.graphics.Flush();
        }

        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.graphics.Dispose();
        }
        #endregion
    }
}
