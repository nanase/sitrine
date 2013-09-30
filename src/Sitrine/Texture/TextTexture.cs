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
    /// <summary>
    /// 変化を伴わない単純な文字列を描画するテクスチャです。
    /// </summary>
    public class TextTexture : Texture
    {
        #region -- Private Fields --
        private readonly TextRenderer renderer;
        private bool updated;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// 描画に使用するレンダラを取得します。
        /// </summary>
        public TextRenderer Renderer { get { return this.renderer; } } 
        #endregion

        #region -- Constructors --
        /// <summary>
        /// テキストレンダラと描画サイズを指定して新しい TextTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="renderer">テキストレンダラ。</param>
        /// <param name="size">描画サイズ。</param>
        public TextTexture(TextRenderer renderer, Size size)
            : base(size)
        {
            if (renderer == null)
                throw new ArgumentNullException();

            this.renderer = renderer;
        }

        /// <summary>
        /// テキストオプションと描画サイズを指定して新しい TextTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="options">テキストオプション。</param>
        /// <param name="size">描画サイズ。</param>
        public TextTexture(TextOptions options, Size size)
            : base(size)
        {
            if (options == null)
                throw new ArgumentNullException();

            this.renderer = new TextRenderer(options, this.BaseBitmap);
        }        
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された文字列を描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        public void Draw(string text)
        {
            this.renderer.DrawString(text, 0.0f, 0.0f);
            this.updated = true;
        }

        /// <summary>
        /// 文字列を指定された座標に描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="x">描画位置の X 座標。</param>
        /// <param name="y">描画位置の Y 座標。</param>
        public void Draw(string text, float x, float y)
        {
            this.renderer.DrawString(text, x, y);
            this.updated = true;
        }

        /// <summary>
        /// 描画を確定させ、画面上に文字列を表示します。
        /// </summary>
        public override void Render()
        {
            if (this.updated)
            {
                this.updated = false;
                this.renderer.Flush();
                Texture.Update(this.ID, this.BaseBitmap);
            }

            base.Render();
        }

        /// <summary>
        /// ビットマップをクリアします。
        /// </summary>
        public void Clear()
        {
            this.renderer.Clear();
            this.updated = true;
        }

        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        public override void Dispose()
        {
            this.renderer.Dispose();
            base.Dispose();
        }
        #endregion
    }
}
