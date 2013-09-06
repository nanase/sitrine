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
using System.Drawing.Imaging;

namespace Sitrine.Utils
{
    public class BitmapController : IDisposable
    {
        #region Private Field
        private readonly BitmapData data;
        private readonly Bitmap bitmap;
        private readonly IntPtr scan0;
        private bool isDisposed;
        #endregion

        #region Public Property
        public unsafe Color this[int x, int y]
        {
            get
            {
                return Color.FromArgb(*((int*)this.scan0 + (this.data.Width * y + x)));
            }
            set
            {
                int* dst = (int*)this.scan0 + (this.data.Width * y + x);
                *dst = value.ToArgb();
            }
        }

        public BitmapData BitmapData { get { return this.data; } }
        public Bitmap BaseBitmap { get { return this.bitmap; } }
        public IntPtr Scan0 { get { return this.scan0; } }
        #endregion

        #region Constructor
        public BitmapController(Bitmap bitmap, ImageLockMode flags)
        {
            this.bitmap = bitmap;
            this.data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), flags, PixelFormat.Format32bppArgb);
            this.scan0 = data.Scan0;
        }
        #endregion

        #region Public Method
        public void Dispose()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException("data");

            this.isDisposed = true;
            this.bitmap.UnlockBits(this.data);
        }
        #endregion
    }
}
