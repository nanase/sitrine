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
    public class TextRender : IDisposable
    {
        #region Private Field
        private static readonly PointF foreOffset;
        private static readonly PointF shadowOffset;

        private readonly Graphics graphics;
        private readonly Bitmap bitmap;
        private readonly TextOptions options;

        private int brushIndex = 0;
        #endregion

        #region Public Property
        public int BrushIndex
        {
            get
            {
                return this.brushIndex;
            }
            set
            {
                if (value < 0 || value >= this.options.Brushes.Length)
                    throw new ArgumentOutOfRangeException();

                this.brushIndex = value;
            }
        }
        #endregion

        #region Constructor
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

        public TextRender(TextOptions options, Bitmap baseBitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException();

            this.bitmap = baseBitmap;
            this.graphics = Graphics.FromImage(baseBitmap);
        }

        public TextRender(TextOptions options, Texture.Texture texture)
        {
            if (texture == null)
                throw new ArgumentNullException();

            this.bitmap = texture.BaseBitmap;
            this.graphics = Graphics.FromImage(texture.BaseBitmap);
        }
        #endregion

        #region Public Method
        #region DrawString(string text, float x, float y)
        public void DrawString(string text)
        {
            this.DrawString(text, this.brushIndex, 0.0f, 0.0f);
        }

        public void DrawString(string text, PointF location)
        {
            this.DrawString(text, this.brushIndex, location.X, location.Y);
        }

        public void DrawString(string text, float x, float y)
        {
            this.DrawString(text, this.brushIndex, x, y);
        }
        #endregion

        #region DrawString(string text, int colorIndex, float x, float y)
        public void DrawString(string text, int colorIndex)
        {
            this.DrawString(text, colorIndex, 0.0f, 0.0f);
        }

        public void DrawString(string text, int colorIndex, PointF location)
        {
            this.DrawString(text, colorIndex, location.X, location.Y);
        }

        public void DrawString(string text, int colorIndex, float x, float y)
        {
            this.graphics.TextRenderingHint = (this.options.Antialiasing) ? TextRenderingHint.AntiAlias : TextRenderingHint.SingleBitPerPixel;

            int i = 0;
            bool fore = (colorIndex > 0 && colorIndex < this.options.Brushes.Length);
            bool shadow = (this.options.DrawShadow && this.options.ShadowIndex > 0 && this.options.ShadowIndex < this.options.Brushes.Length);

            foreach (var line in text.Split('\n'))
            {
                float y_offset = i * (this.options.LineHeight + 1.0f) + y;

                if (shadow)
                    this.graphics.DrawString(line, this.options.Font, this.options.Brushes[this.options.ShadowIndex], TextRender.shadowOffset.X + x, TextRender.shadowOffset.Y + y_offset, this.options.Format);

                if (fore)
                    this.graphics.DrawString(line, this.options.Font, this.options.Brushes[colorIndex], TextRender.foreOffset.X + x, TextRender.foreOffset.Y + y_offset, this.options.Format);

                i++;
            }
        }
        #endregion

        #region DrawChars(string text, float x, float y)
        public SizeF DrawChars(string text)
        {
            return this.DrawChars(text, 0.0f, 0.0f);
        }

        public SizeF DrawChars(string text, PointF location)
        {
            return this.DrawChars(text, location.X, location.Y);
        }

        public SizeF DrawChars(string text, float x, float y)
        {
            return this.DrawChars(text, this.brushIndex, x, y);
        }
        #endregion

        #region DrawChars(string text, int colorIndex, float x, float y)
        public SizeF DrawChars(string text, int colorIndex)
        {
            return this.DrawChars(text, colorIndex, 0.0f, 0.0f);
        }

        public SizeF DrawChars(string text, int colorIndex, PointF location)
        {
            return this.DrawChars(text, colorIndex, location.X, location.Y);
        }

        public SizeF DrawChars(string text, int colorIndex, float x, float y)
        {
            this.graphics.TextRenderingHint = (this.options.Antialiasing) ? TextRenderingHint.AntiAlias : TextRenderingHint.SingleBitPerPixel;

            if (this.options.DrawShadow && this.options.ShadowIndex > 0 && this.options.ShadowIndex < this.options.Brushes.Length)
                this.graphics.DrawString(text, this.options.Font, this.options.Brushes[this.options.ShadowIndex], TextRender.shadowOffset.X + x, TextRender.shadowOffset.Y, this.options.Format);

            if (colorIndex > 0 && colorIndex < this.options.Brushes.Length)
                this.graphics.DrawString(text, this.options.Font, this.options.Brushes[colorIndex], TextRender.foreOffset.X + x, TextRender.foreOffset.Y, this.options.Format);

            return this.graphics.MeasureString(text, this.options.Font, PointF.Empty, this.options.Format);
        }
        #endregion

        public void Clear()
        {
            this.graphics.Clear(Color.Transparent);
        }

        public void Flush()
        {
            this.graphics.Flush();
        }

        public void Dispose()
        {
            this.graphics.Dispose();
        }
        #endregion


    }
}
