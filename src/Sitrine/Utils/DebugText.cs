﻿/* Sitrine - 2D Game Library with OpenTK */

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
using System.Linq;
using Sitrine.Texture;

namespace Sitrine.Utils
{
    /// <summary>
    /// デバッグに必要な情報を表示させるためのクラスです。
    /// </summary>
    public class DebugText : IDisposable
    {
        #region Private Field
        private readonly TextOptions textOptions;
        private readonly SitrineWindow window;
        private readonly TextureList textures;

        private double elapsed = 1.0;
        private int renderCount = 0;
        private readonly TextTexture debugTexture;
        private readonly TextTexture debugTextTexture;
        private TimeSpan processorTimeOld = TimeSpan.Zero;
        private double fps_old = 0.0;

        private long loadCountOld;
        private long updateCountOld;
        private long actionCountOld;
        #endregion

        #region Private Static Field
        private static long loadCount;
        private static long updateCount;
        private static long actionCount;
        #endregion

        #region Public static Property
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

        #region Public Property
        /// <summary>
        /// デバッグ表示を画面上に可視化するかの真偽値を取得または設定します。
        /// </summary>
        public bool IsVisible { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// 各種パラメータを指定して DebugText クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">デバッグ表示のテキストに用いられるテキストオプション。</param>
        /// <param name="window">デバッグ表示が行われるウィンドウ。</param>
        public DebugText(TextOptions options, SitrineWindow window)
        {
            this.textOptions = options;

            this.textOptions.SetSolidBrushes(Color.White, Color.FromArgb(128, Color.Black));
            this.textOptions.ShadowIndex = 1;
            this.textOptions.DrawShadow = true;

            this.debugTexture = new TextTexture(this.textOptions, new Size(window.TargetSize.Width, (int)window.TextOptions.LineHeight));
            this.debugTextTexture = new TextTexture(this.textOptions, new Size(window.TargetSize.Width, (int)window.TextOptions.LineHeight));

            this.window = window;
            this.textures = window.Textures;
            this.fps_old = 60.0;

#if DEBUG
            this.IsVisible = true;
#else
            this.IsVisible = false;
#endif
        }
        #endregion

        #region Public Method
        /// <summary>
        /// デバッグ表示を更新し、テキストを最新のものに変更します。
        /// </summary>
        /// <param name="time">前回実行時までの経過時間。</param>
        public void Update(double time)
        {
            if (!this.IsVisible)
                return;

            this.elapsed += time;
            this.renderCount++;
            if (this.elapsed >= 1.0)
            {
                this.fps_old += (this.window.RenderFrequency - this.fps_old) * 0.1;
                var storyboards = this.window.Storyboards.ToArray();
                this.debugTexture.Draw(string.Format("FPS: {0:f1}({1}), T: {2}, L: {3}, U: {4}, S: {5}, A: {6}, L: {7}, P: {8}, M: {9:f2}MB",
                    this.fps_old,
                    this.renderCount,
                    this.textures.Count,
                    DebugText.loadCount - this.loadCountOld,
                    DebugText.updateCount - this.updateCountOld,

                    storyboards.Length,
                    storyboards.Sum(s => s.ActionCount),
                    storyboards.Sum(s => s.ListenerCount),
                    DebugText.actionCount - this.actionCountOld,
                    GC.GetTotalMemory(false) / (1024.0 * 1024.0)
                    ));

                this.elapsed -= (int)this.elapsed;

                this.renderCount = 0;
                this.loadCountOld = DebugText.loadCount;
                this.updateCountOld = DebugText.updateCount;
                this.actionCountOld = DebugText.actionCount;
            }
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
            this.debugTexture.Dispose();
            this.debugTextTexture.Dispose();
        }

        /// <summary>
        /// デバッグログにテキストを追加します。
        /// </summary>
        /// <param name="message"></param>
        public void SetDebugText(string message)
        {
            this.debugTextTexture.Draw(message, 0f, this.textOptions.LineHeight);
        }
        #endregion

        #region Public Static Method
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
    }
}
