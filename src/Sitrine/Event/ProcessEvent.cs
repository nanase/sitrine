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
    /// ストーリーボードの進行に関わるイベントをスケジューリングします。
    /// </summary>
    public class ProcessEvent : StoryEvent
    {
        #region Constructor
        internal ProcessEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 指定された秒数だけストーリーボードの動作を停止します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="seconds"></param>
        public void Wait(double seconds)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException("seconds");

            if (this.window.TargetUpdateFrequency < 0.0)
                throw new Exception("TargetUpdateFrequency が設定されていません。GameWindow.Run メソッド呼び出し時にパラメータ 'updates_per_second' に値を指定してください。");

            this.storyboard.AddAction(() => this.storyboard.SetWait((int)Math.Round(this.window.TargetUpdateFrequency * seconds)));
        }

        /// <summary>
        /// 指定されたフレーム数だけストーリーボードの動作を停止します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="frame"></param>
        public void WaitFrame(int frame)
        {
            if (frame < 0)
                throw new ArgumentOutOfRangeException("frame");

            this.storyboard.AddAction(() => this.storyboard.SetWait(frame));
        }

        /// <summary>
        /// ストーリーボードの直後のイベントをすべて無視し、ストーリーボードを終了させます。
        /// このメソッドは遅延実行されます。
        /// </summary>
        public void Break()
        {
            this.storyboard.AddAction(this.storyboard.Break);
        }
        #endregion
    }
}
