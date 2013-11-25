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
using Sitrine.Animate;
using Sitrine.Event;

namespace Sitrine.Story
{
    /// <summary>
    /// オブジェクトに対するアニメーションを実現するための機能を提供します。
    /// </summary>
    public class AnimateStoryboard : RenderStoryboard, IExclusiveStory
    {
        #region -- Public Properties --
        /// <summary>
        /// 排他的操作が行われるオブジェクトを取得または設定します。
        /// </summary>
        public object TargetObject { get; set; }

        /// <summary>
        /// 排他的操作が行われるオブジェクトのプロパティ名を取得または設定します。
        /// </summary>
        public string TargetProperty { get; set; }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ストーリーボードが動作する SitrineWindow オブジェクトを指定して
        /// 新しい AnimateStoryboard クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="Window">動作対象の SitrineWindow オブジェクト。</param>
        public AnimateStoryboard(SitrineWindow window)
            : base(window)
        {
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// アクションを指定してこのストーリーボードにアニメーションイベントを構築します。
        /// </summary>
        /// <param name="duration">アニメーションの継続時間。</param>
        /// <param name="easing">使用されるイージング関数。</param>
        /// <param name="initalizer">初期化処理で用いるイニシャライザ。</param>
        /// <param name="iterator">反復処理で用いるイテレータ。</param>
        /// <param name="finalizer">最終処理で用いるファイナライザ。</param>
        public void BuildAnimation(DelaySpan duration, Func<double, double> easing, Action initalizer, Action<float> iterator, Action finalizer)
        {
            if (duration.IsZero())
                return;

            int frame = this.GetFrameCount(duration) / 2;
            easing = easing ?? EasingFunctions.Linear;

            if (initalizer != null)
                Process.Invoke(initalizer);

            for (int i = 0, j = 1; i < frame; i++)
            {
                Process.WaitFrame(1);
                Process.Invoke(() =>
                {
                    float f = (float)easing(j++ / (double)frame);
                    iterator(f);
                });
            }

            if (finalizer != null)
                Process.Invoke(finalizer);
        }
        #endregion
    }
}
