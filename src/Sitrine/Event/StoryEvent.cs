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
    /// ストーリーボードに追加されるイベントの抽象クラスです。
    /// </summary>
    public abstract class StoryEvent<T>
    {
        #region -- Private Fields --
        private DelaySpan delayspan;
        private int? id = null;
        #endregion

        #region -- Protected Fields --
        /// <summary>
        /// イベントが実行されるウィンドウ。この変数は読み取り専用です。
        /// </summary>
        protected readonly SitrineWindow Window;

        /// <summary>
        /// イベントが属するストーリーボード。この変数は読み取り専用です。
        /// </summary>
        protected readonly Storyboard Storyboard;
        #endregion

        #region -- Protected Properties --
        protected int AssignID
        {
            get
            {
                if (!this.id.HasValue)
                    throw new KeyNotSpecifiedException();

                return this.id.Value;
            }
            set
            {
                this.id = value;
            }
        }

        protected T Subclass { private get; set; }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ストーリーボードオブジェクトとそれが実行されるウィンドウオブジェクトを指定して Storyboard クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="Storyboard">対象となる Storyboard オブジェクト。</param>
        /// <param name="Window">ストーリーボードが実行される SitrineWindow オブジェクト。</param>
        public StoryEvent(Storyboard storyboard, SitrineWindow window)
        {
            if (storyboard == null)
                throw new ArgumentNullException("stroyboard");

            if (window == null)
                throw new ArgumentNullException("window");

            this.Storyboard = storyboard;
            this.Window = window;
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 操作対象のテクスチャ ID を指定します。
        /// </summary>
        /// <param name="id">操作対象のテクスチャ ID。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public T ID(int id)
        {
            this.id = id;

            return this.Subclass;
        }

        /// <summary>
        /// 指定されたフレーム数だけ動作を遅延させます。
        /// </summary>
        /// <param name="frames">遅延させるフレーム数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public T Delay(int frames)
        {
            if (frames < 0)
                throw new ArgumentOutOfRangeException("frames");

            this.delayspan.Add(frames);

            return this.Subclass;
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

            this.delayspan.Add(seconds);

            return this.Subclass;
        }
        #endregion

        #region -- Protected Methods --
        protected DelaySpan PopDelaySpan()
        {
            DelaySpan ds = this.delayspan;
            this.delayspan = new DelaySpan();
            return ds;
        }
        #endregion
    }
}
