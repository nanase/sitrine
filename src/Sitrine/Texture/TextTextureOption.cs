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

namespace Sitrine.Texture
{
    public class TextTextureOptions : IDisposable
    {
        #region Private Static Field
        private static readonly StringFormat defaultFormat;
        #endregion

        #region Static Constructor
        static TextTextureOptions()
        {
            defaultFormat = new StringFormat(StringFormat.GenericTypographic);
            defaultFormat.FormatFlags |= StringFormatFlags.NoWrap;
        }
        #endregion

        #region Public Property
        public Font Font { get; private set; }
        public StringFormat Format { get; private set; }
        public int LineHeight { get; private set; }
        #endregion

        #region Contructor
        public TextTextureOptions(Font font, int lineHeight, StringFormat format)
        {
            this.Font = font;
            this.LineHeight = lineHeight;
            this.Format = format;
        }

        public TextTextureOptions(Font font, int lineHeight)
            : this(font, lineHeight, defaultFormat)
        {
        }
        #endregion

        #region Public Method
        public void Dispose()
        {
            this.Font.Dispose();
        }
        #endregion
    }
}
