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
using System.Drawing.Imaging;

namespace Sitrine.Utils
{
    /// <summary>
    /// ビットマップの生データ操作を提供します。
    /// </summary>
    public class BitmapController : IDisposable
    {
        #region -- Private Fields --
        private readonly BitmapData data;
        private readonly Bitmap bitmap;
        private readonly IntPtr scan0;
        private bool isDisposed;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// 座標を指定して Color 構造体を取得または設定します。
        /// </summary>
        /// <param name="x">取得する X 座標。</param>
        /// <param name="y">取得する Y 座標。</param>
        /// <returns>座標が指し示す Color 構造体。</returns>
        public unsafe Color this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0)
                    throw new ArgumentOutOfRangeException();

                if (x >= this.data.Width || y >= this.data.Height)
                    throw new ArgumentOutOfRangeException();

                return Color.FromArgb(*((int*)this.scan0 + (this.data.Width * y + x)));
            }
            set
            {
                if (x < 0 || y < 0)
                    throw new ArgumentOutOfRangeException();

                if (x >= this.data.Width || y >= this.data.Height)
                    throw new ArgumentOutOfRangeException();

                int* dst = (int*)this.scan0 + (this.data.Width * y + x);
                *dst = value.ToArgb();
            }
        }

        /// <summary>
        /// 生データを格納する BitmapData オブジェクトを取得します。
        /// </summary>
        public BitmapData BitmapData { get { return this.data; } }

        /// <summary>
        /// ビットマップの元となった Bitmap オブジェクトを取得します。
        /// </summary>
        public Bitmap BaseBitmap { get { return this.bitmap; } }

        /// <summary>
        /// ビットマップデータの先頭ポインタを取得します。
        /// </summary>
        public IntPtr Scan0 { get { return this.scan0; } }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ビットマップとフラグをもとに BitmapController クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="bitmap">元となるビットマップ。</param>
        /// <param name="flags">メモリロックフラグ。</param>
        public BitmapController(Bitmap bitmap, ImageLockMode flags)
        {
            if (bitmap == null)
                throw new ArgumentNullException();

            this.bitmap = bitmap;
            this.data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), flags, PixelFormat.Format32bppArgb);
            this.scan0 = data.Scan0;
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。ビットマップは解放されません。
        /// </summary>
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
