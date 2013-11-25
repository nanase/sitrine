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
using OpenTK.Graphics;
using Sitrine.Story;
using Sitrine.Utils;

namespace Sitrine.Event
{
    /// <summary>
    /// スクリーンの背景色および前景色に関わるイベントをスケジューリングします。
    /// </summary>
    public class ScreenEvent : StoryEvent<ScreenEvent>
    {
        #region -- Constructors --
        internal ScreenEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.Subclass = this;
        }
        #endregion

        #region -- Public Methods --
        #region ForegroundColor
        /// <summary>
        /// 前景色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent ForegroundColor(Color4 value)
        {
            this.PopDelaySpan().SetDelayAction(this.Storyboard);
            this.Storyboard.AddAction(() => this.Window.ForegroundColor = value);

            return this;
        }

        /// <summary>
        /// 前景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent ForegroundColor(Color4 color, int frames, Func<double, double> easing = null)
        {
            if (frames == 0)
                return this;

            if (frames < 0)
                throw new ArgumentOutOfRangeException("frames");

            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                delay.SetDelayAction(story);
                this.AnimateForeground(story, color, frames, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 前景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="seconds">アニメーションが行われる秒数。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent ForegroundColor(Color4 color, double seconds, Func<double, double> easing = null)
        {
            if (seconds == 0.0)
                return this;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("seconds");

            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                delay.SetDelayAction(story);
                this.AnimateForeground(story, color, story.GetFrameCount(seconds), easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        #endregion

        #region BackgroundColor
        /// <summary>
        /// 背景色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent BackgroundColor(Color4 value)
        {
            this.PopDelaySpan().SetDelayAction(this.Storyboard);
            this.Storyboard.AddAction(() => this.Window.BackgroundColor = value);

            return this;
        }

        /// <summary>
        /// 背景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent BackgroundColor(Color4 color, int frames, Func<double, double> easing = null)
        {
            if (frames == 0)
                return this;

            if (frames < 0)
                throw new ArgumentOutOfRangeException("frames");

            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                delay.SetDelayAction(story);
                this.AnimateBackground(story, color, frames, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 背景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="seconds">アニメーションが行われる秒数。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent BackgroundColor(Color4 color, double seconds, Func<double, double> easing = null)
        {
            if (seconds == 0.0)
                return this;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("seconds");

            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                delay.SetDelayAction(story);
                this.AnimateForeground(story, color, story.GetFrameCount(seconds), easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }
        #endregion

        /// <summary>
        /// 前景色をフェードインし、完全透明にします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent FadeIn(int frames, Func<double, double> easing = null)
        {
            return this.ForegroundColor(new Color4(0, 0, 0, 0), frames, easing);
        }

        /// <summary>
        /// 前景色をフェードインし、完全透明にします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="seconds">アニメーションが行われる秒数。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent FadeIn(double seconds, Func<double, double> easing = null)
        {
            return this.ForegroundColor(new Color4(0, 0, 0, 0), seconds, easing);
        }

        /// <summary>
        /// 前景色をフェードアウトし、完全不透明にします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent FadeOut(int frames, Func<double, double> easing = null)
        {
            return this.ForegroundColor(new Color4(0, 0, 0, 255), frames, easing);
        }

        /// <summary>
        /// 前景色をフェードアウトし、完全不透明にします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="seconds">アニメーションが行われる秒数。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent FadeOut(double seconds, Func<double, double> easing = null)
        {
            return this.ForegroundColor(new Color4(0, 0, 0, 255), seconds, easing);
        }
        #endregion

        #region -- Private Methods --
        private void AnimateForeground(AnimateStoryboard story, Color4 to, int frame, Func<double, double> easing = null)
        {
            story.TargetObject = this.Window;
            story.TargetProperty = "foreground";

            Color4 from = new Color4();

            float dr = 0f, dg = 0f, db = 0f, da = 0f;

            story.BuildAnimation(frame, easing,
                () =>
                {
                    from = this.Window.ForegroundColor;

                    dr = (to.R - from.R);
                    dg = (to.G - from.G);
                    db = (to.B - from.B);
                    da = (to.A - from.A);
                },
                f =>
                {
                    this.Window.ForegroundColor = new Color4((from.R + dr * f).Clamp(1f, 0f),
                                                             (from.G + dg * f).Clamp(1f, 0f),
                                                             (from.B + db * f).Clamp(1f, 0f),
                                                             (from.A + da * f).Clamp(1f, 0f));
                },
                null);
        }

        private void AnimateBackground(AnimateStoryboard story, Color4 to, int frame, Func<double, double> easing = null)
        {
            story.TargetObject = this.Window;
            story.TargetProperty = "background";

            Color4 from = new Color4();

            float dr = 0f, dg = 0f, db = 0f, da = 0f;

            story.BuildAnimation(frame, easing,
                () =>
                {
                    from = this.Window.BackgroundColor;

                    dr = (to.R - from.R);
                    dg = (to.G - from.G);
                    db = (to.B - from.B);
                    da = (to.A - from.A);
                },
                f =>
                {
                    this.Window.BackgroundColor = new Color4((from.R + dr * f).Clamp(1f, 0f),
                                                             (from.G + dg * f).Clamp(1f, 0f),
                                                             (from.B + db * f).Clamp(1f, 0f),
                                                             (from.A + da * f).Clamp(1f, 0f));
                },
                null);
        }
        #endregion
    }
}
