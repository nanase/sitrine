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
using Sitrine.Story;

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
        public Color ForegroundColor
        {
            get
            {
                return this.Window.ForegroundColor;
            }
            set
            {
                this.Storyboard.AddAction(() => this.Window.ForegroundColor = value);
            }
        }

        /// <summary>
        /// 背景色を取得または指定します。
        /// 設定は遅延実行されます。
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return this.Window.BackgroundColor;
            }
            set
            {
                this.Storyboard.AddAction(() => this.Window.BackgroundColor = value);
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
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent AnimateForegroundColor(Color color, int frames)
        {
            if (frames == 0)
                return this;

            if (frames < 0)
                throw new ArgumentOutOfRangeException("frames");

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimateForeground(color, frames);
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
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent AnimateForegroundColor(Color color, double seconds)
        {
            if (seconds == 0.0)
                return this;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("seconds");

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimateForeground(color, story.GetFrameCount(seconds));
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
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent AnimateBackgroundColor(Color color, int frames)
        {
            if (frames == 0)
                return this;

            if (frames < 0)
                throw new ArgumentOutOfRangeException("frames");

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimateBackground(color, frames);
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
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ScreenEvent AnimateBackgroundColor(Color color, double seconds)
        {
            if (seconds == 0.0)
                return this;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("seconds");

            this.Storyboard.AddAction(() =>
            {
                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimateBackground(color, story.GetFrameCount(seconds));
                this.Window.AddStoryboard(story);
            });

            return this;
        }
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
            public void AnimateForeground(Color to, int duration)
            {
                if (duration == 0)
                    return;

                duration /= 2;

                this.targetObject = this.Window;
                this.targetPropery = "foreground";

                Color4 from = new Color4();

                float dr = 0f, dg = 0f, db = 0f, da = 0f;

                Process.Invoke(() =>
                {
                    from = this.Window.ForegroundColor;

                    dr = (to.R / 255f - from.R) / (float)duration;
                    dg = (to.G / 255f - from.G) / (float)duration;
                    db = (to.B / 255f - from.B) / (float)duration;
                    da = (to.A / 255f - from.A) / (float)duration;
                });

                for (int i = 0, j = 1; i < duration; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        this.Window.ForegroundColor = (Color)new Color4(from.R + dr * j, from.G + dg * j, from.B + db * j, from.A + da * j);
                        j++;
                    });
                }
            }

            public void AnimateBackground(Color to, int duration)
            {
                if (duration == 0)
                    return;

                duration /= 2;

                this.targetObject = this.Window;
                this.targetPropery = "background";

                Color4 from = new Color4();

                float dr = 0f, dg = 0f, db = 0f, da = 0f;

                Process.Invoke(() =>
                {
                    from = this.Window.BackgroundColor;

                    dr = (to.R / 255f - from.R) / (float)duration;
                    dg = (to.G / 255f - from.G) / (float)duration;
                    db = (to.B / 255f - from.B) / (float)duration;
                    da = (to.A / 255f - from.A) / (float)duration;
                });

                for (int i = 0, j = 1; i < duration; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        this.Window.BackgroundColor = (Color)new Color4(from.R + dr * j, from.G + dg * j, from.B + db * j, from.A + da * j);
                        j++;
                    });
                }
            }
            #endregion
        }

    }
}
