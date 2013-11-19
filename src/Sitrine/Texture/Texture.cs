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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sitrine.Utils;

namespace Sitrine.Texture
{
    /// <summary>
    /// テクスチャマッピングを用いてビットマップを画面上に描画するためのテクスチャクラスです。
    /// </summary>
    public abstract class Texture : IDisposable
    {
        #region -- Private Fields --
        private Color4 color = Color4.White;
        private PointF position = new PointF();
        private float width, height;
        private float scaleX = 1.0f, scaleY = 1.0f;
        private PointF basePoint = new PointF();
        private bool disposed = false;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// ビットマップの表示上の基準となる幅を取得します。
        /// </summary>
        public float Width
        {
            get { return this.width; }
            set
            {
                this.width = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// ビットマップの表示上の基準となる高さを取得します。
        /// </summary>
        public float Height
        {
            get { return this.height; }
            set
            {
                this.height = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// ビットマップの画面上の位置を取得または設定します。
        /// </summary>
        public PointF Position
        {
            get { return this.position; }
            set
            {
                this.position = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// テクスチャの反射光 (ポリゴンのカラー) を取得または設定します。
        /// </summary>
        public Color4 Color
        {
            get { return this.color; }
            set
            {
                this.color = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// テクスチャの X 軸方向の拡大率を取得または設定します。
        /// </summary>
        public float ScaleX
        {
            get { return this.scaleX; }
            set
            {
                this.scaleX = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// テクスチャの Y 軸方向の拡大率を取得または設定します。
        /// </summary>
        public float ScaleY
        {
            get { return this.scaleY; }
            set
            {
                this.scaleY = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// 拡大や回転の基点となるテクスチャ上の座標を取得または設定します。
        /// この座標はピクセル値ではなく、テクスチャの各辺の長さを 1.0 としたときの 0.0 から 1.0 の割合で表現します。
        /// </summary>
        public PointF BasePoint
        {
            get { return this.basePoint; }
            set
            {
                this.basePoint = value;

                this.RequestRecompile();
            }
        }

        /// <summary>
        /// 実際に表示されるテクスチャの幅を取得します。
        /// </summary>
        public float AcctualWidth
        {
            get { return this.width * this.scaleX; }
        }

        /// <summary>
        /// 実際に表示されるテクスチャの高さを取得します。
        /// </summary>
        public float AcctualHeight
        {
            get { return this.height * this.scaleY; }
        }

        /// <summary>
        /// 処理をコンパイルせずにバッファに適用するかの真偽値を取得または設定します。
        /// 描画するたびにコンパイル処理が発生するとパフォーマンスが低下します。
        /// 頻繁な変更を行わない場合、このプロパティを false にするとパフォーマンスは向上します。
        /// </summary>
        public bool NoCompile { get; set; }
        #endregion

        #region -- Protected Properties --
        /// <summary>
        /// 再コンパイルの必要性を表す真偽値を取得します。
        /// </summary>
        protected bool RequiredRecompile { get; set; }

        /// <summary>
        /// ディスプレイリストの ID を取得または設定します。
        /// </summary>
        protected int ListID { get; set; }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 画面上にテクスチャを表示します。
        /// </summary>
        public virtual void Render()
        {
            if (this.NoCompile)
            {
                this.Execute();
            }
            else
            {
                if (this.RequiredRecompile)
                    this.Compile(ListMode.CompileAndExecute);
                else
                    GL.CallList(this.ListID);
            }
        }
        #endregion

        #region -- Protected Methods --
        /// <summary>
        /// テクスチャをロードし、画面上に表示できる状態にします。
        /// </summary>
        protected virtual void Load()
        {
            DebugText.IncrementLoadCount();
        }

        /// <summary>
        /// テクスチャデータを更新します。
        /// </summary>
        protected virtual void Update()
        {
            DebugText.IncrementUpdateCount();
        }

        /// <summary>
        /// 再コンパイルを要求します。
        /// </summary>
        protected void RequestRecompile()
        {
            if (!this.RequiredRecompile)
            {
                if (this.ListID != -1)
                {
                    GL.DeleteLists(this.ListID, 1);
                    this.ListID = -1;
                }

                this.RequiredRecompile = true;
            }
        }

        /// <summary>
        /// ディスプレイリストに対するコンパイルを実行します。
        /// </summary>
        /// <param name="mode">コンパイル時のモード。</param>
        protected virtual void Compile(ListMode mode = ListMode.Compile)
        {
            if (this.ListID != -1)
                GL.DeleteLists(this.ListID, 1);

            this.ListID = GL.GenLists(1);
            GL.NewList(this.ListID, mode);
            {
                this.Execute();
            }
            GL.EndList();

            this.RequiredRecompile = false;
        }

        /// <summary>
        /// 描画処理を実行します。
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// このオブジェクトによって使用されているアンマネージリソースを解放し、オプションでマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は true。アンマネージリソースだけを解放する場合は false。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.ListID != -1)
                        GL.DeleteLists(this.ListID, 1);
                }

                this.ListID = -1;

                this.disposed = true;
            }
        }
        #endregion

        #region -- Destructors --
        ~Texture()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
