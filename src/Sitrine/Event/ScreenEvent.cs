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
using System.Drawing;
using OpenTK.Graphics;
using Sitrine.Animate;
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
        #region Setter
        /// <summary>
        /// 前景色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent ForegroundColor(Color value)
        {
            this.PopDelaySpan().SetDelayAction(this.Storyboard);
            this.Storyboard.AddAction(() => this.Window.ForegroundColor = value);

            return this;
        }

        /// <summary>
        /// 背景色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent BackgroundColor(Color value)
        {
            this.PopDelaySpan().SetDelayAction(this.Storyboard);
            this.Storyboard.AddAction(() => this.Window.BackgroundColor = value);

            return this;
        }
        #endregion

        #region Action
        /// <summary>
        /// 前景色を指定された色までアニメーション表示します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">アニメーション完了時の色。</param>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent AnimateForegroundColor(Color color, int frames, Func<double, double> easing = null)
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
                story.AnimateForeground(color, frames, easing);
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
        public ScreenEvent AnimateForegroundColor(Color color, double seconds, Func<double, double> easing = null)
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
                story.AnimateForeground(color, story.GetFrameCount(seconds), easing);
                this.Window.AddStoryboard(story);
            });

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
        public ScreenEvent AnimateBackgroundColor(Color color, int frames, Func<double, double> easing = null)
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
                story.AnimateBackground(color, frames, easing);
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
        public ScreenEvent AnimateBackgroundColor(Color color, double seconds, Func<double, double> easing = null)
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
                story.AnimateBackground(color, story.GetFrameCount(seconds), easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 前景色をフェードインし、完全透明にします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="frames">アニメーションが行われるフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent FadeIn(int frames, Func<double, double> easing = null)
        {
            return this.AnimateForegroundColor(Color.FromArgb(0, Color.Black), frames, easing);
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
            return this.AnimateForegroundColor(Color.FromArgb(0, Color.Black), seconds, easing);
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
            return this.AnimateForegroundColor(Color.Black, frames, easing);
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
            return this.AnimateForegroundColor(Color.Black, seconds, easing);
        }
        #endregion
        #endregion

        class AnimateStoryboard : RenderStoryboard, IExclusiveStory
        {
            #region -- Private Fields --
            private object targetObject;
            private string targetPropery;
            #endregion

            #region -- Public Properties --
            public object TargetObject
            {
                get { return this.targetObject; }
            }

            public string TargetProperty
            {
                get { return this.targetPropery; }
            }
            #endregion

            #region -- Constructors --
            public AnimateStoryboard(SitrineWindow window)
                : base(window)
            {
            }
            #endregion

            #region -- Public Methods --
            public void AnimateForeground(Color to, int duration, Func<double, double> easing = null)
            {
                if (duration == 0)
                    return;

                duration /= 2;
                easing = easing ?? EasingFunctions.Linear;

                this.targetObject = this.Window;
                this.targetPropery = "foreground";

                Color4 from = new Color4();

                float dr = 0f, dg = 0f, db = 0f, da = 0f;

                Process.Invoke(() =>
                {
                    from = this.Window.ForegroundColor;

                    dr = (to.R / 255f - from.R);
                    dg = (to.G / 255f - from.G);
                    db = (to.B / 255f - from.B);
                    da = (to.A / 255f - from.A);
                });

                for (int i = 0, j = 1; i < duration; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        float f = (float)easing(j++ / (double)duration);
                        this.Window.ForegroundColor = (Color)new Color4((from.R + dr * f).Clamp(1f, 0f),
                                                                        (from.G + dg * f).Clamp(1f, 0f),
                                                                        (from.B + db * f).Clamp(1f, 0f),
                                                                        (from.A + da * f).Clamp(1f, 0f));
                    });
                }
            }

            public void AnimateBackground(Color to, int duration, Func<double, double> easing = null)
            {
                if (duration == 0)
                    return;

                duration /= 2;
                easing = easing ?? EasingFunctions.Linear;

                this.targetObject = this.Window;
                this.targetPropery = "background";

                Color4 from = new Color4();

                float dr = 0f, dg = 0f, db = 0f, da = 0f;

                Process.Invoke(() =>
                {
                    from = this.Window.BackgroundColor;

                    dr = (to.R / 255f - from.R);
                    dg = (to.G / 255f - from.G);
                    db = (to.B / 255f - from.B);
                    da = (to.A / 255f - from.A);
                });

                for (int i = 0, j = 1; i < duration; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        float f = (float)easing(j++ / (double)duration);
                        this.Window.BackgroundColor = (Color)new Color4((from.R + dr * f).Clamp(1f, 0f),
                                                                        (from.G + dg * f).Clamp(1f, 0f),
                                                                        (from.B + db * f).Clamp(1f, 0f),
                                                                        (from.A + da * f).Clamp(1f, 0f));
                    });
                }
            }
            #endregion
        }

    }
}
