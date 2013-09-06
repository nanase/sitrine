﻿/* Sitrine - 2D Game Library with OpenTK */

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
    public class FontLoader : IDisposable
    {
        #region Private Field
        private readonly PrivateFontCollection fontCollection;
        private readonly FontFamily family;
        #endregion

        #region Public Property
        public FontFamily Family { get { return this.family; } }
        #endregion

        #region Constructor
        public FontLoader(string filename)
        {
            this.fontCollection = new PrivateFontCollection();
            this.fontCollection.AddFontFile(filename);

            if (this.fontCollection.Families.Length == 0)
                throw new Exception("指定されたファイルからフォントが見つかりません。");

            this.family = this.fontCollection.Families[0];
        }
        #endregion

        #region Public Method
        public void Dispose()
        {
            this.family.Dispose();
            this.fontCollection.Dispose();
        }
        #endregion
    }
}
