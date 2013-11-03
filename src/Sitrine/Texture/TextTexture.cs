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

using System.Drawing;
using Sitrine.Utils;

namespace Sitrine.Texture
{
    /// <summary>
    /// 変化を伴わない単純な文字列を描画するテクスチャです。
    /// </summary>
    public class TextTexture : TextTextureBase
    {
         #region -- Constructors --
        /// <summary>
        /// テキストレンダラと描画サイズを指定して新しい TextTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="renderer">テキストレンダラ。</param>
        /// <param name="size">描画サイズ。</param>
        public TextTexture(TextRenderer renderer, Size size)
            : base(renderer, size)
        {
        }

        /// <summary>
        /// テキストオプションと描画サイズを指定して新しい TextTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="options">テキストオプション。</param>
        /// <param name="size">描画サイズ。</param>
        public TextTexture(TextOptions options, Size size)
            : base(options, size)
        {
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された文字列を描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        public　override void Draw(string text)
        {
            this.Renderer.DrawString(text, 0.0f, 0.0f);
            this.updated = true;
        }

        /// <summary>
        /// 文字列を指定された座標に描画します。
        /// </summary>
        /// <param name="text">描画される文字列。</param>
        /// <param name="x">描画位置の X 座標。</param>
        /// <param name="y">描画位置の Y 座標。</param>
        public override void Draw(string text, float x, float y)
        {
            this.Renderer.DrawString(text, x, y);
            this.updated = true;
        }        
        #endregion        
    }
}
