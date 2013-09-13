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

namespace Sitrine.Texture
{
    public class TextTexture : Texture
    {
        #region Protected Field
        protected readonly TextTextureOptions options;
        protected readonly Graphics g;
        protected readonly PointF forePoint;
        protected readonly PointF shadowPoint;
        protected string text;

        protected SolidBrush foreBrush = new SolidBrush(System.Drawing.Color.White);
        protected SolidBrush shadowBrush = new SolidBrush(System.Drawing.Color.Black);
        #endregion

        #region Public Property
        public string Text { get { return this.text; } }
        public TextTextureOptions Options { get { return this.options; } }

        public Color ForeColor
        {
            get
            {
                return this.foreBrush.Color;
            }
            set
            {
                if (this.foreBrush != null)
                    this.foreBrush.Dispose();
                this.foreBrush = new SolidBrush(value);
            }
        }

        public Color ShadowColor
        {
            get
            {
                return this.shadowBrush.Color;
            }
            set
            {
                if (this.shadowBrush != null)
                    this.shadowBrush.Dispose();
                this.shadowBrush = new SolidBrush(value);
            }
        }
        #endregion

        #region Constructor
        public TextTexture(TextTextureOptions options, Size size, bool antialias)
            : base(new Bitmap(size.Width, size.Height))
        {
            this.options = options;
            this.g = Graphics.FromImage(this.bitmap);
            this.g.TextRenderingHint = antialias ? TextRenderingHint.AntiAlias : TextRenderingHint.SingleBitPerPixel;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                this.forePoint = new PointF(1, -1);
                this.shadowPoint = new PointF(2, 0);
            }
            else
            {
                this.forePoint = new PointF(0, -2);
                this.shadowPoint = new PointF(1, -1);
            }

            Texture.Load(this.id, this.bitmap);
        }
        #endregion

        #region Public Method
        public void Draw(string text)
        {
            if (text != this.text)
            {
                this.text = text;
                this.g.Clear(this.clearColor);
                this.DrawString(text);
                this.g.Flush();
                Texture.Update(this.id, this.bitmap);
            }
        }

        public void Draw(string text, float x, float y)
        {
            this.text = text;
            this.g.Clear(this.clearColor);
            this.DrawString(text, x, y);
            this.g.Flush();
            Texture.Update(this.id, this.bitmap);
        }

        public void Clear()
        {
            this.g.Clear(this.clearColor);
            this.g.Flush();
            Texture.Update(this.id, this.bitmap);
        }

        public override void Dispose()
        {
            this.g.Dispose();

            base.Dispose();
        }
        #endregion

        #region Protected Method
        protected void DrawString(string text)
        {
            int i = 0;
            foreach (var line in text.Split('\n'))
            {
                float y_offset = i * (this.options.LineHeight + 1);

                this.g.DrawString(line, this.options.Font, this.shadowBrush, this.shadowPoint.X, this.shadowPoint.Y + y_offset, this.options.Format);
                this.g.DrawString(line, this.options.Font, this.foreBrush, this.forePoint.X, this.forePoint.Y + y_offset, this.options.Format);

                i++;
            }
        }

        protected void DrawString(string text, float x, float y)
        {
            int i = 0;
            foreach (var line in text.Split('\n'))
            {
                float y_offset = i * (this.options.LineHeight + 1) + y;

                this.g.DrawString(line, this.options.Font, this.shadowBrush, this.shadowPoint.X + x, this.shadowPoint.Y + y_offset, this.options.Format);
                this.g.DrawString(line, this.options.Font, this.foreBrush, this.forePoint.X + x, this.forePoint.Y + y_offset, this.options.Format);

                i++;
            }
        }
        #endregion
    }
}
