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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sitrine.Audio;
using Sitrine.Texture;
using Sitrine.Utils;

namespace Sitrine
{
    /// <summary>
    /// GameWindow に各種補助機能を追加したウィンドウを定義します。
    /// </summary>
    public class SitrineWindow : GameWindow
    {
        #region -- Private Fields --
        private Color backgroundColor = Color.Black;
        private Color foregroundColor = Color.FromArgb(0);
        #endregion

        #region -- Protected Fields --
        protected readonly TextureList textures;
        protected readonly MusicPlayer music;        
        protected readonly DebugText debugText;
        protected readonly List<Storyboard> stories;
        protected readonly TextOptions textOptions;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// テキストの表示に用いられるテキストオプションを取得します。
        /// </summary>
        public TextOptions TextOptions { get { return this.textOptions; } }

        /// <summary>
        /// 音楽の再生を管理するオブジェクトを取得します。
        /// </summary>
        public MusicPlayer Music { get { return this.music; } }

        /// <summary>
        /// テクスチャのリストを取得します。
        /// </summary>
        public TextureList Textures { get { return this.textures; } }

        /// <summary>
        /// 実行されるストーリーボードを表す列挙子を取得します。
        /// </summary>
        public IEnumerable<Storyboard> Storyboards { get { return this.stories; } }

        /// <summary>
        /// 描画サイズを取得します。描画サイズはウィンドウの表示サイズとは異なります。
        /// </summary>
        public Size TargetSize { get; protected set; }

        /// <summary>
        /// ウィンドウの背景色を取得または設定します。
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                GL.ClearColor(value);
                this.backgroundColor = value;
            }
        }

        /// <summary>
        /// ウィンドウの前景色を取得または設定します。
        /// </summary>
        public Color ForegroundColor
        {
            get
            {
                return this.foregroundColor;
            }
            set
            {
                this.foregroundColor = value;
            }
        }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ウィンドウオプションを指定して SitrineWindow クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">ウィンドウオプション。</param>
        public SitrineWindow(WindowOptions options)
            : base(options.WindowSize.Width, options.WindowSize.Height, GraphicsMode.Default, options.Title)
        {
            this.music = new MusicPlayer();
            this.textures = new TextureList();           
            this.TargetSize = options.TargetSize;
            this.stories = new List<Storyboard>();
            this.textOptions = options.TextOptions;

            this.debugText = new DebugText(options.DebugTextOptions, this);
            Trace.Listeners.Add(new DebugTextListener(this.debugText));     
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// デバッグ表示をトグルします。
        /// </summary>
        public void ToggleDebugVisibility()
        {
            this.debugText.IsVisible = !this.debugText.IsVisible;
        }
        #endregion

        #region -- Protected Methods --
        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {
            if (this.Keyboard[OpenTK.Input.Key.Escape])
                this.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(this.backgroundColor);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            this.debugText.Update(0.0);

            this.music.Play();
        }

        protected override void OnUnload(EventArgs e)
        {
            this.music.Stop();
            this.music.Dispose();
            this.textures.Dispose();
            this.debugText.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(0.0, this.TargetSize.Width, this.TargetSize.Height, 0.0, -1.0, 1.0);

            int factor = Math.Min(this.ClientSize.Width / TargetSize.Width, this.ClientSize.Height / TargetSize.Height);
            if (factor == 0)
                GL.Viewport(TargetSize);
            else
                GL.Viewport((this.Width - TargetSize.Width * factor) / 2, (this.Height - TargetSize.Height * factor) / 2,
                    TargetSize.Width * factor, TargetSize.Height * factor);
        }


        protected void ProcessBeforeRender(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        protected void ProcessAfterRender(FrameEventArgs e)
        {
            this.textures.Render();

            GL.PushMatrix();
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(this.foregroundColor);
                GL.Rect(0, 0, this.TargetSize.Width, this.TargetSize.Height);
                GL.Enable(EnableCap.Texture2D);
            }
            GL.PopMatrix();

            this.debugText.Render();
            this.SwapBuffers();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.ProcessBeforeRender(e);
            this.ProcessAfterRender(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            foreach (var story in this.stories)
                story.Update();

            this.debugText.Update(e.Time);
        }
        #endregion
    }
}
