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
    public class TextOptions
    {
        #region Private Field
        private float lineHeight;
        private Brush[] brushes;
        private Font font;
        #endregion

        #region Public Property
        public float LineHeight
        {
            get
            {
                return this.lineHeight;
            }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentException();

                if (value < 0.0)
                    throw new ArgumentOutOfRangeException();

                this.lineHeight = value;
            }
        }

        public Brush[] Brushes
        {
            get
            {
                return this.brushes;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (value.Length == 0)
                    throw new ArgumentException();

                this.brushes = value;
            }
        }

        public bool Antialiasing { get; set; }

        public Font Font
        {
            get
            {
                return this.font;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                this.font = value;
            }
        }
        #endregion

        #region Public Method
        public void SetSolidBrushes(params Color[] colors)
        {
            if (colors == null)
                throw new ArgumentNullException();

            if (colors.Length == 0)
                throw new ArgumentException();

            this.brushes = new Brush[colors.Length];

            for (int i = 0, j = colors.Length; i < j; i++)
                this.brushes[i] = new SolidBrush(colors[i]);
        }
        #endregion
    }
}
