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
using Sitrine.Utils;

namespace Sitrine.Texture
{
    public abstract class TextTextureBase : BitmapTexture
    {
        #region -- Private Fields --
        private bool disposed = false;
        #endregion

        #region -- Protected Fields --
        protected bool updated;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// 描画に使用するレンダラを取得します。
        /// </summary>
        public TextRenderer Renderer { get; private set; }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// テキストレンダラと描画サイズを指定して新しい TextTextureBase クラスの派生クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="renderer">テキストレンダラ。</param>
        /// <param name="size">描画サイズ。</param>
        public TextTextureBase(TextRenderer renderer, Size size)
            : base(size)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            this.Renderer = renderer;
        }

        /// <summary>
        /// テキストオプションと描画サイズを指定して新しい TextTextureBase クラスの派生クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="options">テキストオプション。</param>
        /// <param name="size">描画サイズ。</param>
        public TextTextureBase(TextOptions options, Size size)
            : base(size)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            this.Renderer = new TextRenderer(options, this.Loader);
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された文字列を描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        public abstract void Draw(string text);

        /// <summary>
        /// 文字列を指定された座標に描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="x">描画位置の X 座標。</param>
        /// <param name="y">描画位置の Y 座標。</param>
        public abstract void Draw(string text, float x, float y);

        /// <summary>
        /// 描画を確定させ、画面上に文字列を表示します。
        /// </summary>
        public override void Render()
        {
            if (this.updated)
            {
                this.updated = false;
                this.Renderer.Flush();
                this.Update();
            }

            base.Render();
        }

        /// <summary>
        /// ビットマップをクリアします。
        /// </summary>
        public void Clear()
        {
            this.Renderer.Clear();
            this.updated = true;
        }

        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region -- Protected Methods --
        /// <summary>
        /// このオブジェクトによって使用されているアンマネージリソースを解放し、オプションでマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は true。アンマネージリソースだけを解放する場合は false。</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.Renderer != null)
                        this.Renderer.Dispose();
                }

                this.Renderer = null;

                this.disposed = true;
            }
        }
        #endregion

        #region -- Destructors --
        ~TextTextureBase()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
