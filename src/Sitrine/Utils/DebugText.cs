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
using System.Drawing;
using System.Linq;
using OpenTK;
using Sitrine.Texture;

namespace Sitrine.Utils
{
    /// <summary>
    /// デバッグに必要な情報を表示させるためのクラスです。
    /// </summary>
    public class DebugText : IDisposable
    {
        #region -- Private Fields --
        private readonly TextOptions textOptions;
        private readonly SitrineWindow window;
        private readonly TextureList textures;
        private readonly LinkedList<TextItem> textQueue;

        private double elapsed = 1.0;
        private double elapsed_fps = 2.0;
        private TextTexture debugTexture;
        private TextTexture debugTextTexture;
        private double fps_now = 0.0;
        private bool textUpdated = false;
        private DateTime firstQueueTime;

        private long loadCountOld;
        private long updateCountOld;
        private long actionCountOld;

        private bool disposed = false;
        #endregion

        #region -- Private Static Fields --
        private static long loadCount;
        private static long updateCount;
        private static long actionCount;
        #endregion

        #region -- Public Static Properties --
        /// <summary>
        /// テクスチャをロードした回数を取得します。
        /// </summary>
        public static long LoadCount { get { return DebugText.loadCount; } }

        /// <summary>
        /// テクスチャを更新した回数を取得します。
        /// </summary>
        public static long UpdateCount { get { return DebugText.updateCount; } }

        /// <summary>
        /// ストーリーボードのアクションを実行した回数を取得します。
        /// </summary>
        public static long ActionCount { get { return DebugText.actionCount; } }
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// デバッグ表示を画面上に可視化するかの真偽値を取得または設定します。
        /// </summary>
        public bool IsVisible { get; set; }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// 各種パラメータを指定して DebugText クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">デバッグ表示のテキストに用いられるテキストオプション。</param>
        /// <param name="Window">デバッグ表示が行われるウィンドウ。</param>
        public DebugText(TextOptions options, SitrineWindow window)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (window == null)
                throw new ArgumentNullException("window");

            this.textOptions = options;

            this.textOptions.SetSolidBrushes(Color.White, Color.FromArgb(128, Color.Black), Color.Green, Color.Yellow, Color.Red);
            this.textOptions.ShadowIndex = 1;
            this.textOptions.DrawShadow = true;

            this.debugTexture = new TextTexture(this.textOptions, new Size(window.TargetSize.Width, (int)options.LineHeight + 1));
            this.debugTextTexture = new TextTexture(this.textOptions, new Size(window.TargetSize.Width, (int)(options.LineHeight + 1) * 4 + 1));
            this.debugTextTexture.Position = new Vector2d(0.0, options.LineHeight + 1.0);

            this.window = window;
            this.textures = window.Textures;

            this.textQueue = new LinkedList<TextItem>();

#if DEBUG
            this.IsVisible = true;
#else
            this.IsVisible = false;
#endif
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// デバッグ表示を更新し、テキストを最新のものに変更します。
        /// </summary>
        /// <param name="time">前回実行時までの経過時間。</param>
        public void Update(double time)
        {
            if (!this.IsVisible)
                return;

            this.UpdateDebug(time);

            this.UpdateDebugText();
        }

        /// <summary>
        /// デバッグ表示を画面上に表示します。
        /// </summary>
        public void Render()
        {
            if (!this.IsVisible)
                return;

            this.debugTexture.Render();
            this.debugTextTexture.Render();
        }

        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region -- Public Static Methods --
        /// <summary>
        /// テクスチャのロードカウンタをインクリメントします。
        /// </summary>
        public static void IncrementLoadCount()
        {
            DebugText.loadCount++;
        }

        /// <summary>
        /// テクスチャのロードカウンタを指定された数だけ加算します。
        /// </summary>
        /// <param name="value">加算する値。</param>
        public static void IncrementLoadCount(long value)
        {
            DebugText.loadCount += (value < 0 ? 0 : value);
        }

        /// <summary>
        /// テクスチャの更新カウンタをインクリメントします。
        /// </summary>
        public static void IncrementUpdateCount()
        {
            DebugText.updateCount++;
        }

        /// <summary>
        /// テクスチャの更新カウンタを指定された数だけ加算します。
        /// </summary>
        /// <param name="value">加算する値。</param>
        public static void IncrementUpdateCount(long value)
        {
            DebugText.updateCount += (value < 0 ? 0 : value);
        }

        /// <summary>
        /// ストーリーボードのアクション実行回数をインクリメントします。
        /// </summary>
        public static void IncrementActionCount()
        {
            DebugText.actionCount++;
        }

        /// <summary>
        /// ストーリーボードのアクション実行回数を指定された数だけ加算します。
        /// </summary>
        /// <param name="value">加算する値。</param>
        public static void IncrementActionCount(long value)
        {
            DebugText.actionCount += (value < 0 ? 0 : value);
        }
        #endregion

        #region -- Protected Methods --
        /// <summary>
        /// このオブジェクトによって使用されているアンマネージリソースを解放し、オプションでマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は true。アンマネージリソースだけを解放する場合は false。</param>
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.debugTexture != null)
                        this.debugTexture.Dispose();

                    if (this.debugTextTexture != null)
                        this.debugTextTexture.Dispose();
                }

                this.debugTexture = null;
                this.debugTextTexture = null;

                this.disposed = true;
            }
        }
        #endregion

        #region -- Internal Methods --
        internal void SetDebugText(string message)
        {
            this.textUpdated = true;
            this.textQueue.AddFirst(new TextItem(message, 0));
        }

        internal void SetDebugInfoText(string message)
        {
            this.textUpdated = true;
            this.textQueue.AddFirst(new TextItem(message, 2));
        }

        internal void SetDebugWarningText(string message)
        {
            this.textUpdated = true;
            this.textQueue.AddFirst(new TextItem(message, 3));
        }

        internal void SetDebugErrorText(string message)
        {
            this.textUpdated = true;
            this.textQueue.AddFirst(new TextItem(message, 4));
        }
        #endregion

        #region -- Private Methods --
        private void UpdateDebug(double time)
        {
            this.elapsed += time;
            this.elapsed_fps += time;

            if (this.elapsed_fps >= 2.0)
            {
                this.fps_now = this.window.RenderFrequency;
                this.elapsed_fps = 0.0;
            }

            if (this.elapsed >= 0.25)
            {
                var storyboards = this.window.Storyboards.ToArray();

                this.debugTexture.Clear();
                this.debugTexture.Draw(string.Format("FPS: {0:f1}({1}), T: {2}, L: {3}, U: {4}, S: {5}, A: {6}, L: {7}, P: {8}, M: {9:f2}MB",
                    this.fps_now,
                    (int)Math.Round(1.0 / time),
                    this.textures.Count,
                    DebugText.loadCount - this.loadCountOld,
                    DebugText.updateCount - this.updateCountOld,

                    storyboards.Length,
                    storyboards.Sum(s => s.ActionCount),
                    storyboards.Sum(s => s.ListenerCount),
                    DebugText.actionCount - this.actionCountOld,
                    GC.GetTotalMemory(false) / (1024.0 * 1024.0)
                    ));

                this.elapsed = 0.0;
                this.loadCountOld = DebugText.loadCount;
                this.updateCountOld = DebugText.updateCount;
                this.actionCountOld = DebugText.actionCount;
            }
        }

        private void UpdateDebugText()
        {
            var now = DateTime.Now;

            if (this.textUpdated)
            {
                this.textUpdated = false;
                this.debugTextTexture.Clear();


                var item = this.textQueue.First;

                for (int i = 0, j = 0; item != null; i++)
                {
                    var next = item.Next;

                    if ((i >= 4) || (now - item.Value.QueuedTime).TotalSeconds > 10.0)
                        this.textQueue.Remove(item);
                    else
                    {
                        this.debugTextTexture.Renderer.BrushIndex = item.Value.BrushIndex;
                        this.debugTextTexture.Draw(item.Value.Text, 0, j * (this.debugTextTexture.Renderer.Options.LineHeight + 1));
                        j++;

                        this.firstQueueTime = item.Value.QueuedTime;
                    }

                    item = next;
                }
            }
            else if ((now - this.firstQueueTime).TotalSeconds > 10.0)
            {
                var item = this.textQueue.First;

                for (int i = 0; item != null; i++)
                {
                    var next = item.Next;

                    if ((now - item.Value.QueuedTime).TotalSeconds > 10.0)
                        this.textQueue.Remove(item);

                    item = next;
                }

                this.textUpdated = true;
            }
        }
        #endregion

        #region -- Destructors --
        ~DebugText()
        {
            this.Dispose(false);
        }
        #endregion

        class TextItem
        {
            #region -- Properties
            public int BrushIndex { get; private set; }

            public string Text { get; private set; }

            public DateTime QueuedTime { get; private set; }
            #endregion

            #region -- Constructor --
            public TextItem(string text, int brushIndex)
            {
                this.Text = text;
                this.BrushIndex = brushIndex;
                this.QueuedTime = DateTime.Now;
            }
            #endregion
        }
    }
}
