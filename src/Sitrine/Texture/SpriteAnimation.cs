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
using System.IO;
using OpenTK.Graphics.OpenGL;
using Sitrine.Utils;

namespace Sitrine.Texture
{
    /// <summary>
    /// 同一の画像から領域を分割し、アニメーション表示させるためのテクスチャです。
    /// </summary>
    public class SpriteAnimation : BitmapTexture, IAnimationTexture
    {
        #region -- Private Fields --
        private int x, y;
        private float s_width, s_height;
        private int interval, counter;
        private int[] lists;

        private int nowFrame;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// テクスチャの横方向の画像数を取得します。
        /// </summary>
        public int CountX { get { return this.x; } }

        /// <summary>
        /// テクスチャの縦方向の画像数を取得します。
        /// </summary>
        public int CountY { get { return this.y; } }

        /// <summary>
        /// 画像の更新フレーム間隔を取得または設定します。
        /// </summary>
        public int Interval
        {
            get { return this.interval; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                else
                    this.interval = value;
            }
        }

        /// <summary>
        /// アニメーションの総フレーム数を取得します。
        /// </summary>
        public int FrameCount { get { return this.x * this.y; } }

        /// <summary>
        /// アニメーションの 1 フレームの幅を取得します。
        /// </summary>
        public int FrameWidth { get { return this.Loader.BaseBitmap.Width / this.x; } }

        /// <summary>
        /// アニメーションの 1 フレームの高さを取得します。
        /// </summary>
        public int FrameHeight { get { return this.Loader.BaseBitmap.Height / this.y; } }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ビットマップを指定して新しい SpriteAnimation クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="bitmap">ビットマップローダ。</param>
        /// <param name="countX">横方向の分割数。</param>
        /// <param name="countY">縦方向の分割数。</param>
        public SpriteAnimation(BitmapLoader bitmap, int countX, int countY)
            : base(bitmap)
        {
            this.Initalize(countX, countY);
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// フレームを更新し、必要であればアニメーションのフレームを 1 つ進めます。
        /// </summary>
        /// <returns>フレームが更新されたとき true、それ以外のとき false。</returns>
        public bool UpdateFrame()
        {
            if (++this.counter > this.interval)
            {
                if (++this.nowFrame >= this.FrameCount)
                    this.nowFrame = 0;

                this.counter = 0;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// アニメーションを開始します。
        /// </summary>
        public void Start()
        {
            this.counter = 0;
        }

        /// <summary>
        /// 画面にテクスチャを描画します。
        /// </summary>
        public override void Render()
        {
            if (this.NoCompile)
            {
                this.Execute(this.nowFrame);
            }
            else
            {
                if (this.RequiredRecompile)
                    this.Compile(ListMode.CompileAndExecute);
                else
                    GL.CallList(this.lists[nowFrame]);
            }
        }
        #endregion

        #region -- Protected Methods --
        protected override void Compile(ListMode mode = ListMode.Compile)
        {
            for (int i = 0; i < this.FrameCount; i++)
            {
                this.lists[i] = GL.GenLists(1);
                GL.NewList(this.lists[i], ListMode.Compile);
                {
                    this.Execute(i);
                }
                GL.EndList();
            }

            this.RequiredRecompile = false;
        }
        #endregion

        #region -- Private Methods --
        private void Execute(int frameIndex)
        {
            int x = frameIndex % this.x;
            int y = frameIndex / this.x;

            GL.PushMatrix();

            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);
            GL.Translate(this.Position.X, this.Position.Y, 0.0f);
            GL.Scale(this.Width * this.ScaleX, this.Height * this.ScaleY, 1.0f);
            GL.Color4(this.Color);

            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(x * this.s_width, y * this.s_height); GL.Vertex2(0.0f, 0.0f);
                GL.TexCoord2(x * this.s_width, (y + 1) * this.s_height); GL.Vertex2(0.0f, 1.0f);
                GL.TexCoord2((x + 1) * this.s_width, (y + 1) * this.s_height); GL.Vertex2(1.0f, 1.0f);
                GL.TexCoord2((x + 1) * this.s_width, y * this.s_height); GL.Vertex2(1.0f, 0.0f);
            }
            GL.End();
            GL.PopMatrix();
        }

        private void Initalize(int countX, int countY)
        {
            if (countX <= 0)
                throw new ArgumentOutOfRangeException("xCount");

            if (countY <= 0)
                throw new ArgumentOutOfRangeException("yCount");

            this.x = countX;
            this.y = countY;

            this.Width = this.FrameWidth;
            this.Height = this.FrameHeight;

            this.s_width = 1.0f / (float)countX;
            this.s_height = 1.0f / (float)countY;

            this.lists = new int[this.FrameCount];

            this.interval = 4;
        }
        #endregion
    }
}
