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

namespace Sitrine.Story
{
    /// <summary>
    /// ストーリーボードの機能を拡張し、同一のアクションが指定回数分だけループするストーリーボードを提供します。
    /// </summary>
    public class LoopStoryboard : Storyboard
    {
        #region -- Private Fields --
        private readonly List<Action> actionPool = new List<Action>();
        private int loopCount = -1;
        private int nowCount;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// ループする回数を取得または設定します。-1 を設定すると無限にループを行います。
        /// </summary>
        public int LoopCount
        {
            get
            {
                return this.loopCount;
            }
            protected set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("value");

                this.loopCount = value;
            }
        }
        #endregion

        #region -- Constructor --
        /// <summary>
        /// ストーリーボードが動作する SitrineWindow オブジェクトを指定して新しい Storyboard クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="Window">動作対象の SitrineWindow オブジェクト。</param>
        public LoopStoryboard(SitrineWindow window)
            : base(window)
        {
            this.AutoEnd = false;
        }
        #endregion

        #region -- Public Static Methods --
        /// <summary>
        /// 指定したストーリーを実行する LoopStoryboard クラスのインスタンスを作成します。
        /// </summary>
        /// <param name="window">ストーリーボードが実行されるウィンドウオブジェクト。</param>
        /// <param name="storyevents">ストーリーイベントが記述されたメソッド。</param>
        /// <param name="loopCount">ループの回数。-1 を設定すると無限にループを行います。</param>
        /// <returns>生成されたストーリーボード。</returns>
        public static LoopStoryboard Create(SitrineWindow window, Action<Storyboard> storyevents, int loopCount = -1)
        {
            LoopStoryboard loopstory = new LoopStoryboard(window);
            storyevents(loopstory);
            loopstory.LoopCount = loopCount;
            loopstory.StartLooping();

            return loopstory;
        }
        #endregion

        #region -- Protected Methods --
        /// <summary>
        /// ストーリーボードにキューイングされているアクションを確定し、ループを開始します。
        /// </summary>
        protected void StartLooping()
        {
            this.nowCount = 0;
            this.actionPool.AddRange(this.Actions);
        }
        #endregion

        #region -- Internal Methods --
        internal override void Update()
        {
            if (this.Actions.Count == 0)
            {
                if (this.loopCount == -1 || ++this.nowCount < this.loopCount)
                    foreach (var action in this.actionPool)
                        this.Actions.AddFirst(action);
                else
                    this.AutoEnd = true;
            }

            base.Update();
        }
        #endregion
    }
}
