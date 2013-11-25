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
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sitrine.Audio;
using Sitrine.Story;
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
        private readonly DebugText debugText;

        private readonly TextureList textures;
        private readonly MusicPlayer music;
        private readonly TextOptions textOptions;

        private readonly List<Storyboard> stories;
        private readonly List<RenderStoryboard> renderStories;
        private readonly List<Storyboard> reservedStories;
        private readonly List<Storyboard> removingStories;

        private readonly SolidTexture foreground;

        private Color4 backgroundColor = Color4.Black;
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
        public IEnumerable<Storyboard> Storyboards { get { return this.stories.Union(this.renderStories); } }

        /// <summary>
        /// 描画サイズを取得します。描画サイズはウィンドウの表示サイズとは異なります。
        /// </summary>
        public Size TargetSize { get; protected set; }

        /// <summary>
        /// ウィンドウの背景色を取得または設定します。
        /// </summary>
        public Color4 BackgroundColor
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
        public Color4 ForegroundColor
        {
            get
            {
                return this.foreground.Color;
            }
            set
            {
                this.foreground.Color = value;
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
            if (options == null)
                throw new ArgumentNullException("options");

            this.music = new MusicPlayer(new MusicOptions());
            this.textures = new TextureList();
            this.TargetSize = options.TargetSize;
            this.textOptions = options.TextOptions;
            this.foreground = new SolidTexture();
            this.foreground.Color = new Color4(0, 0, 0, 0);

            this.stories = new List<Storyboard>();
            this.renderStories = new List<RenderStoryboard>();
            this.reservedStories = new List<Storyboard>();
            this.removingStories = new List<Storyboard>();

            this.debugText = new DebugText(options.DebugTextOptions, this);
            Trace.Listeners.Add(new DebugTextListener(this.debugText));
            Trace.WriteLine("Window", "Init");
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

        /// <summary>
        /// ストーリーボードを追加します。
        /// </summary>
        /// <param name="Storyboard">追加されるストーリーボード。</param>
        public void AddStoryboard(Storyboard storyboard)
        {
            this.reservedStories.Add(storyboard);
        }

        /// <summary>
        /// ストーリーボードを実行対象から除外します。
        /// </summary>
        /// <param name="Storyboard">除外されるストーリーボード。</param>
        public void RemoveStoryboard(Storyboard storyboard)
        {
            this.removingStories.Add(storyboard);
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

            this.foreground.Width = this.TargetSize.Width;
            this.foreground.Height = this.TargetSize.Height;
        }


        protected void ProcessBeforeRender(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        protected void ProcessAfterRender(FrameEventArgs e)
        {
            foreach (var story in this.renderStories)
                story.Update();

            this.textures.Render();
            this.foreground.Render();
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

            if (this.removingStories.Count > 0)
            {
                foreach (var story in this.removingStories)
                {
                    story.Dispose();

                    if (story is RenderStoryboard)
                        this.renderStories.Remove((RenderStoryboard)story);
                    else
                        this.stories.Remove(story);
                }

                this.removingStories.Clear();
            }

            this.DequeueStory();

            this.debugText.Update(e.Time);
        }
        #endregion

        #region -- Private Methods --
        private void DequeueStory()
        {
            for (int i = 0; i < this.reservedStories.Count; i++)
            {
                Storyboard story = this.reservedStories[i];

                if (!(story is IExclusiveStory)
                    || (!this.stories.Any(s => s is IExclusiveStory && Storyboard.CheckExclusive((IExclusiveStory)story, (IExclusiveStory)s))
                      && !this.renderStories.Any(s => s is IExclusiveStory && Storyboard.CheckExclusive((IExclusiveStory)story, (IExclusiveStory)s))))
                {
                    if (story is RenderStoryboard)
                        this.renderStories.Add((RenderStoryboard)story);
                    else
                        this.stories.Add(story);

                    this.reservedStories.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion
    }
}
