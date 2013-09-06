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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sitrine.Audio;
using Sitrine.Texture;
using Sitrine.Utils;
using System;
using System.Drawing;

namespace Sitrine
{
    public class SitrineWindow : GameWindow
    {
        #region Protected Field
        protected readonly TextureList textures;
        protected readonly MusicPlayer music;
        protected readonly Size targetSize;
        protected readonly DebugText debugText;
        #endregion

        #region Constructor
        public SitrineWindow(WindowOption option)
            : base(option.WindowSize.Width, option.WindowSize.Height, GraphicsMode.Default, option.Title)
        {
            this.music = new MusicPlayer();
            this.textures = new TextureList();
            this.debugText = new DebugText(option.DebugTextFontFile, option.DebugTextFontSize, this, this.textures);
            this.targetSize = option.TargetSize;
        }
        #endregion

        #region Protected Method
        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {
            if (this.Keyboard[OpenTK.Input.Key.Escape])
                this.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Black);
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

            GL.Ortho(0.0, this.targetSize.Width, this.targetSize.Height, 0.0, -1.0, 1.0);

            int factor = Math.Min(this.ClientSize.Width / targetSize.Width, this.ClientSize.Height / targetSize.Height);
            if (factor == 0)
                GL.Viewport(targetSize);
            else
                GL.Viewport((this.Width - targetSize.Width * factor) / 2, (this.Height - targetSize.Height * factor) / 2,
                    targetSize.Width * factor, targetSize.Height * factor);
        }

        protected void ProcessBeforeRender(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        protected void ProcessAfterRender(FrameEventArgs e)
        {
            this.textures.Show();
            this.debugText.Update(e.Time);
            this.SwapBuffers();
        }
        #endregion
    }
}
