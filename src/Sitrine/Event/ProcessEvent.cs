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
        #region -- Constructors --
        internal ProcessEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された秒数だけストーリーボードの動作を停止します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="seconds">停止する秒数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent Wait(double seconds)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException("seconds");

            this.Storyboard.AddAction(() => this.Storyboard.SetWait(this.Storyboard.GetFrameCount(seconds)));

            return this;
        }

        /// <summary>
        /// 指定されたフレーム数だけストーリーボードの動作を停止します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="frame">停止するフレーム数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent WaitFrame(int frame)
        {
            if (frame < 0)
                throw new ArgumentOutOfRangeException("frame");

            this.Storyboard.AddAction(() => this.Storyboard.SetWait(frame));

            return this;
        }

        /// <summary>
        /// ストーリーボードの直後のイベントをすべて無視し、ストーリーボードを終了させます。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent Break()
        {
            this.Storyboard.AddAction(this.Storyboard.Break);

            return this;
        }

        /// <summary>
        /// 指定された処理を予約し、遅延実行させます。
        /// </summary>
        /// <param name="action">予約する処理。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent Invoke(Action action)
        {
            this.Storyboard.AddAction(action);

            return this;
        }

        /// <summary>
        /// 新しいストーリーボードを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="newStoryboard">開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent Fork(Storyboard newStoryboard)
        {
            this.Storyboard.AddAction(() => this.Window.AddStoryboard(newStoryboard));

            return this;
        }

        /// <summary>
        /// 条件を満たした場合にのみ新しいストーリーボードを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="condition">条件判定処理。</param>
        /// <param name="newStoryboard">開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent ForkIf(Func<bool> condition, Storyboard newStoryboard)
        {
            this.Storyboard.AddAction(() =>
            {
                if (condition())
                    this.Window.AddStoryboard(newStoryboard);
            });

            return this;
        }

        /// <summary>
        /// 条件により開始するストーリーボードを選択し、開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="condition">条件判定処理。</param>
        /// <param name="storyOnTrue">条件が true のときに開始されるストーリーボード。</param>
        /// <param name="storyOnFalse">条件が false のときに開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent ForkIf(Func<bool> condition, Storyboard storyOnTrue, Storyboard storyOnFalse)
        {
            this.Storyboard.AddAction(() => this.Window.AddStoryboard(condition() ? storyOnTrue : storyOnFalse));

            return this;
        }

        /// <summary>
        /// 条件を満たさない場合にのみ新しいストーリーボードを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="condition">条件判定処理。</param>
        /// <param name="newStoryboard">開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent ForkElseIf(Func<bool> condition, Storyboard newStoryboard)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!condition())
                    this.Window.AddStoryboard(newStoryboard);
            });

            return this;
        }

        /// <summary>
        /// ストーリーボードを終了させ、指定したストーリーボードに動作を切り替えます。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="newStoryboard">開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent Yield(Storyboard newStoryboard)
        {
            this.Storyboard.AddAction(() =>
            {
                this.Window.AddStoryboard(newStoryboard);
                this.Storyboard.Break();
            });

            return this;
        }

        /// <summary>
        /// 条件を満たした場合にのみストーリーボードを終了させ、指定したストーリーボードに動作を切り替えます。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="condition">条件判定処理。</param>
        /// <param name="newStoryboard">開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent YieldIf(Func<bool> condition, Storyboard newStoryboard)
        {
            this.Storyboard.AddAction(() =>
            {
                if (condition())
                {
                    this.Window.AddStoryboard(newStoryboard);
                    this.Storyboard.Break();
                }
            });

            return this;
        }

        /// <summary>
        /// ストーリーボードを終了させ、条件により指定したストーリーボードに動作を切り替えます。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="condition">条件判定処理。</param>
        /// <param name="storyOnTrue">条件が true のときに開始されるストーリーボード。</param>
        /// <param name="storyOnFalse">条件が false のときに開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent YieldIf(Func<bool> condition, Storyboard storyOnTrue, Storyboard storyOnFalse)
        {
            this.Storyboard.AddAction(() =>
            {
                this.Window.AddStoryboard(condition() ? storyOnTrue : storyOnFalse);
                this.Storyboard.Break();
            });

            return this;
        }

        /// <summary>
        /// 条件を満たさない場合にのみストーリーボードを終了させ、指定したストーリーボードに動作を切り替えます。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="condition">条件判定処理。</param>
        /// <param name="newStoryboard">開始されるストーリーボード。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public ProcessEvent YieldElseIf(Func<bool> condition, Storyboard newStoryboard)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!condition())
                {
                    this.Window.AddStoryboard(newStoryboard);
                    this.Storyboard.Break();
                }
            });

            return this;
        }
        #endregion
    }
}
