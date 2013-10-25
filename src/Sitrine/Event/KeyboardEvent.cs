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
using OpenTK.Input;

namespace Sitrine.Event
{
    /// <summary>
    /// キーボードに関連するイベントをスケジューリングします。
    /// </summary>
    public class KeyboardEvent : StoryEvent
    {
        #region -- Private Fields --
        private bool keyUpFlag;

        private Key[] okKeys = new[] { Key.Enter, Key.KeypadEnter, Key.Space, Key.Z };
        private Key[] cancelKeys = new[] { Key.BackSpace, Key.X };
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// OK (肯定) 入力に用いる Key 列挙体の配列を取得または設定します。
        /// </summary>
        public Key[] OKKeys
        {
            get
            {
                return this.okKeys;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0)
                    throw new ArgumentException();

                this.okKeys = value;
            }
        }

        /// <summary>
        /// キャンセル (否定) 入力に用いる Key 列挙体の配列を取得または設定します。
        /// </summary>
        public Key[] CancelKeys
        {
            get
            {
                return this.cancelKeys;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0)
                    throw new ArgumentException();

                this.cancelKeys = value;
            }
        }
        #endregion

        #region -- Constructors --
        internal KeyboardEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// OK キーが入力されるまでストーリーボードを停止します。
        /// このメソッドはストーリーボードが開始している場合に限り遅延実行します。それ以外のとき、即時に実行を開始します。
        /// </summary>
        /// <param name="callback">動作が再開したときに呼び出される処理。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public KeyboardEvent WaitForOK(Action callback = null)
        {
            if (this.Storyboard.State != StoryboardState.Started)
                this.Listen(callback, this.okKeys);
            else
                this.Storyboard.AddAction(() => this.Listen(callback, this.okKeys));

            return this;
        }

        /// <summary>
        /// キャンセル キーが入力されるまでストーリーボードを停止します。
        /// このメソッドはストーリーボードが開始している場合に限り遅延実行します。それ以外のとき、即時に実行を開始します。
        /// </summary>
        /// <param name="callback">動作が再開したときに呼び出される処理。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public KeyboardEvent WaitForCancel(Action callback = null)
        {
            if (this.Storyboard.State != StoryboardState.Started)
                this.Listen(callback, this.cancelKeys);
            else
                this.Storyboard.AddAction(() => this.Listen(callback, this.cancelKeys));

            return this;
        }

        /// <summary>
        /// 指定されたキーのいずれかが入力されるまでストーリーボードを停止します。
        /// このメソッドはストーリーボードが開始している場合に限り遅延実行します。それ以外のとき、即時に実行を開始します。
        /// </summary>
        /// <param name="callback">動作が再開した時に呼び出される処理。</param>
        /// <param name="keys">入力を待つキー。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public KeyboardEvent WaitFor(Action callback = null, params Key[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException();

            if (keys.Length == 0)
                throw new ArgumentException();

            if (this.Storyboard.State != StoryboardState.Started)
                this.Listen(callback, keys);
            else
                this.Storyboard.AddAction(() => this.Listen(callback, keys));

            return this;
        }
        #endregion

        #region -- Private Methods --
        private void Listen(Action callback, Key[] keys)
        {
            this.keyUpFlag = false;
            this.Storyboard.Pause();

            if (callback == null)
                this.Storyboard.AddListener(() => this.ListenKeys(keys));
            else
                this.Storyboard.AddListener(() => this.ListenKeysWithCallback(callback, keys));
        }

        private bool ListenKeys(params Key[] keys)
        {
            if (!this.keyUpFlag)
            {
                if (!this.CheckAny(keys))
                    this.keyUpFlag = true;

                return false;
            }
            else if (this.CheckAny(keys))
            {
                this.Storyboard.Start();
                return true;
            }
            else
                return false;
        }

        private bool ListenKeysWithCallback(Action callback, Key[] keys)
        {
            if (!this.keyUpFlag)
            {
                if (!this.CheckAny(keys))
                    this.keyUpFlag = true;

                return false;
            }
            else if (this.CheckAny(keys))
            {
                callback();
                this.Storyboard.Start();
                return true;
            }
            else
                return false;
        }

        private bool CheckAny(params Key[] keys)
        {
            for (int i = 0, j = keys.Length; i < j; i++)
                if (this.Window.Keyboard[keys[i]])
                    return true;

            return false;
        }

        private bool CheckAll(params Key[] keys)
        {
            for (int i = 0, j = keys.Length; i < j; i++)
                if (!this.Window.Keyboard[keys[i]])
                    return false;

            return true;
        }
        #endregion
    }
}
