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

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sitrine.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Sitrine.Texture
{
    public class Texture : IDisposable
    {
        #region Private Field
        private int listId = -1;
        #endregion

        #region Protected Field
        protected readonly int id;
        protected Bitmap bitmap;
        protected Color4 color = Color4.White;
        protected PointF position = new PointF();
        #endregion

        #region Public Property
        public int ID { get { return this.id; } }
        public Bitmap BaseBitmap { get { return this.bitmap; } }

        public int Width { get { return this.bitmap.Width; } }
        public int Height { get { return this.bitmap.Height; } }

        public PointF Position
        {
            get { return this.position; }
            set
            {
                this.position = value;

                if (this.listId != -1)
                {
                    GL.DeleteLists(this.listId, 1);
                    this.listId = -1;
                }
            }
        }

        public Color4 Color
        {
            get { return this.color; }
            set
            {
                this.color = value;

                if (this.listId != -1)
                {
                    GL.DeleteLists(this.listId, 1);
                    this.listId = -1;
                }
            }
        }
        #endregion

        #region Constructor
        public Texture(Bitmap bitmap)
        {
            this.bitmap = bitmap;

            GL.GenTextures(1, out this.id);
            Texture.Load(this.id, bitmap);
        }

        public Texture(string filename)
            : this(new Bitmap(filename))
        {
        }

        public Texture(Stream stream)
            : this(new Bitmap(stream))
        {
        }

        public Texture(Size size)
            : this(new Bitmap(size.Width, size.Height))
        {
        }
        #endregion

        #region Public Method
        public virtual void Dispose()
        {
            int id = this.id;
            GL.DeleteTextures(1, ref id);

            if (this.listId != -1)
                GL.DeleteLists(this.listId, 1);

            this.bitmap.Dispose();
        }

        public virtual void Render()
        {
            if (this.listId == -1)
                this.Compile(ListMode.CompileAndExecute);
            else
                GL.CallList(this.listId);
        }
        #endregion

        #region Public Static Method
        public static void Update(int id, Bitmap bitmap)
        {
            DebugText.IncrementUpdateCount();
            GL.BindTexture(TextureTarget.Texture2D, id);

            using (BitmapController bc = new BitmapController(bitmap, ImageLockMode.ReadOnly))
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bc.Scan0);
        }

        public static void Load(int id, Bitmap bitmap)
        {
            DebugText.IncrementLoadCount();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            using (BitmapController bc = new BitmapController(bitmap, ImageLockMode.ReadOnly))
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bc.Scan0);
        }

        public static void Load(int id, string filename)
        {
            using (Bitmap bitmap = new Bitmap(filename))
                Texture.Load(id, bitmap);
        }
        #endregion

        #region Private Method
        private void Compile(ListMode mode = ListMode.Compile)
        {
            if (this.listId != -1)
                GL.DeleteLists(this.listId, 1);

            this.listId = GL.GenLists(1);
            GL.NewList(this.listId, mode);
            {
                GL.PushMatrix();

                GL.BindTexture(TextureTarget.Texture2D, this.ID);
                GL.Translate(this.position.X, this.position.Y, 0.0f);
                GL.Scale(this.bitmap.Width, this.bitmap.Height, 1.0);
                GL.Color4(this.color);

                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0.0f, 1.0f);
                    GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1.0f, 1.0f);
                    GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1.0f, 0.0f);
                    GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0.0f, 0.0f);
                }
                GL.End();
                GL.PopMatrix();
            }
            GL.EndList();
        }
        #endregion
    }
}
