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
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace Sitrine.Event
{
    /// <summary>
    /// スクリーンの背景色および前景色に関わるイベントをスケジューリングします。
    /// </summary>
    public class ScreenEvent : StoryEvent
    {
        #region -- Public Properties --
        /// <summary>
        /// 前景色を取得または指定します。
        /// 設定は遅延実行されます。
        /// </summary>
        public Color4 ForegroundColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 背景色を取得または指定します。
        /// 設定は遅延実行されます。
        /// </summary>
        public Color4 BackgroundColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region -- Constructors --
        internal ScreenEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 前景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        public void AnimateForegroundColor(Color4 color, int frames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 前景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="seconds">アニメーションが行われる秒数。</param>
        public void AnimateForegroundColor(Color4 color, double seconds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 背景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        public void AnimateBackgroundColor(Color4 color, int frames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 背景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="seconds">アニメーションが行われる秒数。</param>
        public void AnimateBackgroundColor(Color4 color, double seconds)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
