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
    class TextRender : IDisposable
    {
        #region Private Field
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
            this.DrawString(text, 0.0f, 0.0f);
        }

        public void DrawString(string text, PointF location)
        {
            this.DrawString(text, location.X, location.Y);
        }

        public void DrawString(string text, float x, float y)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        #endregion

        #region DrawChars(string text, float x, float y)
        public void DrawChars(string text)
        {
            this.DrawChars(text, 0.0f, 0.0f);
        }

        public void DrawChars(string text, PointF location)
        {
            this.DrawChars(text, location.X, location.Y);
        }

        public void DrawChars(string text, float x, float y)
        {
            throw new NotImplementedException();
        }
        #endregion

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
