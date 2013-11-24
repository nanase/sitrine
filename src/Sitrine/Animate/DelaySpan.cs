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

namespace Sitrine.Event
{
    /// <summary>
    /// ストーリーイベントが次のイベントを実行するまでの遅延時間を格納します。
    /// </summary>
    public struct DelaySpan
    {
        #region -- Private Fields --
        private int delayFrames;
        private double delaySeconds;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// 遅延されるフレーム数を取得します。
        /// </summary>
        public int DelayFrames { get { return this.delayFrames; } }

        /// <summary>
        /// 遅延される秒数を取得します。
        /// </summary>
        public double DelaySeconds { get { return this.delaySeconds; } }
        #endregion

        #region -- Constructor --
        /// <summary>
        /// 遅延されるフレーム数を指定して新しい DelaySpan 構造体のインスタンスを初期化します。
        /// </summary>
        /// <param name="delayFrames">遅延されるフレーム数。</param>
        public DelaySpan(int delayFrames)
        {
            this.delayFrames = delayFrames;
            this.delaySeconds = 0.0;
        }

        /// <summary>
        /// 遅延される秒数を指定して新しい DelaySpan 構造体のインスタンスを初期化します。
        /// </summary>
        /// <param name="delaySeconds">遅延される秒数。</param>
        public DelaySpan(double delaySeconds)
        {
            this.delayFrames = 0;
            this.delaySeconds = delaySeconds;
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 遅延されるフレーム数を加算します。
        /// </summary>
        /// <param name="frames">加算されるフレーム数。</param>
        public void Add(int frames)
        {
            if (frames < 0)
                throw new ArgumentOutOfRangeException("frames");

            this.delayFrames += frames;
        }

        /// <summary>
        /// 遅延される秒数を加算します。
        /// </summary>
        /// <param name="seconds">加算される秒数。</param>
        public void Add(double seconds)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException("seconds");

            this.delaySeconds += seconds;
        }

        /// <summary>
        /// 指定されたストーリーボードにウェイトイベントを指定します。
        /// </summary>
        /// <param name="story"></param>
        public void SetDelayAction(Storyboard story)
        {
            if (story == null)
                throw new ArgumentNullException("story");

            var process = story.Process;

            if (this.delaySeconds != 0.0)
                process.Wait(this.delaySeconds);

            if (this.delayFrames != 0)
                process.WaitFrame(this.delayFrames);
        }

        /// <summary>
        /// このインスタンスの値がゼロに等しいかの真偽値を返します。
        /// </summary>
        /// <returns>ゼロの場合は true、それ以外の場合は false。</returns>
        public bool IsZero()
        {
            return this.delayFrames == 0 && this.delaySeconds == 0.0;
        }
        #endregion

        #region -- Public Static Methods --
        public static implicit operator DelaySpan(int frames)
        {
            return new DelaySpan(frames);
        }

        public static implicit operator DelaySpan(double seconds)
        {
            return new DelaySpan(seconds);
        }
        #endregion
    }
}
