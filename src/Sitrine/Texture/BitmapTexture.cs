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
using OpenTK.Graphics.OpenGL;
using Sitrine.Utils;

namespace Sitrine.Texture
{
    /// <summary>
    /// ビットマップ画像を用いてポリゴンにテクスチャマッピングを施します。
    /// </summary>
    public class BitmapTexture : Texture
    {
        #region -- Private Fields --
        private BitmapLoader loader;
        private bool disposed = false;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// OpenGL で使われているテクスチャの ID を取得します。
        /// </summary>
        public int TextureID { get; private set; }

        /// <summary>
        /// 表示されるビットマップオブジェクトを取得します。
        /// </summary>
        public BitmapLoader Loader { get { return this.loader; } }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ビットマップを指定して新しい BitmapTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="loader">関連付けられるビットマップ。</param>
        public BitmapTexture(BitmapLoader loader)
        {
            if (loader == null)
                throw new ArgumentNullException("bitmap");

            this.loader = loader;

            int textureId;
            GL.GenTextures(1, out textureId);
            this.TextureID = textureId;

            this.Width = loader.BaseBitmap.Width;
            this.Height = loader.BaseBitmap.Height;

            this.Load();
        }

        /// <summary>
        /// サイズを指定して空のビットマップを作成し、新しい BitmapTexture クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="size">ビットマップのサイズ。</param>
        public BitmapTexture(Size size)
            : this(new BitmapLoader(new Bitmap(size.Width, size.Height)))
        {
        }
        #endregion

        #region -- Public Methods --
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
        /// テクスチャをロードし、画面上に表示できる状態にします。
        /// </summary>
        protected override void Load()
        {
            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.Loader.BaseBitmap.Width, this.Loader.BaseBitmap.Height,
                          0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, this.Loader.Scan0);

            base.Load();
        }

        /// <summary>
        /// テクスチャデータを更新します。
        /// </summary>
        protected override void Update()
        {
            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, this.Loader.BaseBitmap.Width, this.Loader.BaseBitmap.Height,
                             OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, this.Loader.Scan0);

            base.Update();
        }

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
                    if (this.TextureID != -1)
                        GL.DeleteTexture(this.TextureID);
                }

                this.TextureID = -1;

                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// 描画処理を実行します。
        /// </summary>
        protected override void Execute()
        {
            GL.PushMatrix();

            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);
            GL.Translate(this.Position.X, this.Position.Y, 0.0f);
            GL.Scale(this.Width, this.Height, 1.0f);
            GL.Color4(this.Color);

            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0.0f, 0.0f);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0.0f, 1.0f);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1.0f, 1.0f);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1.0f, 0.0f);
            }
            GL.End();
            GL.PopMatrix();
        }
        #endregion

         #region -- Destructors --
        ~BitmapTexture()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
