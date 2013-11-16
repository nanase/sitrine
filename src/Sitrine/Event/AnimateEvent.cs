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
    /// アニメーションに必要なディレイなどのメソッドを提供します。
    /// </summary>
    /// <typeparam name="T">サブクラス。</typeparam>
    public abstract class AnimateEvent<T> : StoryEvent
    {
        #region -- Private Fields --
        private int delayFrame;
        private double delaySeconds;
        #endregion

        #region -- Protected Fields --
        protected T subclass;
        #endregion

        #region -- Protected Properties --
        protected int DelayFrames { get { return this.delayFrame; } }

        protected double DelaySeconds { get { return this.delaySeconds; } }
        #endregion

        #region -- Constructor --
        /// <summary>
        /// ストーリーボードオブジェクトとそれが実行されるウィンドウオブジェクトを指定して Storyboard クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="Storyboard">対象となる Storyboard オブジェクト。</param>
        /// <param name="Window">ストーリーボードが実行される SitrineWindow オブジェクト。</param>
        public AnimateEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定されたフレーム数だけ動作を遅延させます。
        /// </summary>
        /// <param name="frame">遅延させるフレーム数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public T Delay(int frame)
        {
            if (frame < 0)
                throw new ArgumentOutOfRangeException("frame");

            this.delayFrame += frame;

            return this.subclass;
        }

        /// <summary>
        /// 指定された秒数だけ動作を遅延させます。
        /// </summary>
        /// <param name="seconds">遅延させる秒数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public T Delay(double seconds)
        {
            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("seconds");

            this.delaySeconds += seconds;

            return this.subclass;
        }
        #endregion

        #region -- Protected Methods --
        protected void ResetDelay()
        {
            this.delayFrame = 0;
            this.delaySeconds = 0.0;
        }
        #endregion
    }
}
