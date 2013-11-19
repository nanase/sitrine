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

using OpenTK.Graphics.OpenGL;

namespace Sitrine.Texture
{
    /// <summary>
    /// 画像を使わずに単一色を表すテクスチャです。
    /// </summary>
    public class SolidTexture : Texture
    {
        #region -- Protected Methods --
        /// <summary>
        /// 描画処理を実行します。
        /// </summary>
        protected override void Execute()
        {
            GL.PushMatrix();
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Translate(this.Position.X + this.BasePoint.X, this.Position.Y + this.BasePoint.Y, 0.0f);
                GL.Rotate(this.RotateZ, 0f, 0f, 1.0f);
                GL.Scale(this.Width * this.ScaleX, this.Height * this.ScaleY, 1.0f);
                GL.Translate(-this.BasePoint.X, -this.BasePoint.Y, 0.0f);
                GL.Color4(this.Color);
                GL.Rect(0f, 0f, 1f, 1f);
                GL.Enable(EnableCap.Texture2D);
            }
            GL.PopMatrix();
        }
        #endregion
    }
}
