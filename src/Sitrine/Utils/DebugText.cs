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
using Sitrine.Texture;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Sitrine.Utils
{
    public class DebugText : IDisposable
    {
        #region Private Field
        private readonly FontLoader font;
        private readonly TextTextureOptions textOptions;
        private readonly GameWindow window;
        private readonly TextureList textures;

        private double elapsed = 1.0;
        private int renderCount = 0;
        private readonly TextTexture debugTexture;
        private readonly Process current;
        private TimeSpan processorTimeOld = TimeSpan.Zero;

        private long loadCountOld;
        private long updateCountOld;
        #endregion

        #region Private Static Field
        private static long loadCount;
        private static long updateCount;
        #endregion

        #region Public static Property
        public static long LoadCount { get { return DebugText.loadCount; } }

        public static long UpdateCount { get { return DebugText.updateCount; } }
        #endregion

        #region Public Property
        public bool ShowDebug { get; set; }
        #endregion

        #region Constructor
        public DebugText(string fontfile, int size, GameWindow window, TextureList textures)
        {
            this.font = new FontLoader(fontfile);
            this.textOptions = new TextTextureOptions(new Font(this.font.Family, size, GraphicsUnit.Pixel), size);

            this.current = Process.GetCurrentProcess();
            this.debugTexture = new TextTexture(this.textOptions, new Size(320, 60), false);

            this.window = window;
            this.textures = textures;

#if DEBUG
            this.ShowDebug = true;
#else
            this.ShowDebug = false;
#endif
        }
        #endregion

        #region Public Method
        public void Update(double time)
        {
            if (!this.ShowDebug)
                return;

            this.debugTexture.Show();

            this.elapsed += time;
            this.renderCount++;
            if (this.elapsed >= 1.0)
            {
                this.debugTexture.Draw(string.Format("FPS: {0:f1}({1}), T: {2}, L: {3}, U: {4}, M: {5:f2}MB, P: {6:p1}",
                    this.window.RenderFrequency,
                    this.renderCount,
                    this.textures.Count,
                    DebugText.loadCount - this.loadCountOld,
                    DebugText.updateCount - this.updateCountOld,
                    this.current.WorkingSet64 / (1024.0 * 1024.0),
                    this.GetUsage()));

                this.elapsed -= (int)this.elapsed;
                this.renderCount = 0;
                this.loadCountOld = DebugText.loadCount;
                this.updateCountOld = DebugText.updateCount;
            }
        }

        public void Dispose()
        {
            this.font.Dispose();
            this.textOptions.Dispose();
            this.debugTexture.Dispose();
            this.current.Dispose();
        }
        #endregion

        #region Public Static Method
        public static void IncrementLoadCount()
        {
            DebugText.loadCount++;
        }

        public static void IncrementUpdateCount()
        {
            DebugText.updateCount++;
        }
        #endregion

        #region Private Method
        private double GetUsage()
        {
            TimeSpan now = this.current.TotalProcessorTime;
            double usage = (now - this.processorTimeOld).TotalSeconds / this.elapsed;
            this.processorTimeOld = now;
            return usage;
        }
        #endregion
    }
}
