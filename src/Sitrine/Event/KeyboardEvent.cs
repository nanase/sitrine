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
        #region Private Field
        private bool keyUpFlag;

        private Key[] okKeys = new[] { Key.Enter, Key.KeypadEnter, Key.Space, Key.Z };
        private Key[] cancelKeys = new[] { Key.BackSpace, Key.X };
        #endregion

        #region Public Property
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

        #region Constructor
        internal KeyboardEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region Public Method
        /// <summary>
        /// OK キーが入力されるまでストーリーボードを停止します。
        /// このメソッドはストーリーボードが開始している場合に限り遅延実行します。それ以外のとき、即時に実行を開始します。
        /// </summary>
        /// <param name="callback">動作が再開したときに呼び出される処理。</param>
        public void WaitForOK(Action callback = null)
        {
            if (this.storyboard.State != StoryboardState.Started)
                this.Listen(callback, this.okKeys);
            else
                this.storyboard.AddAction(() => this.Listen(callback, this.okKeys));
        }

        /// <summary>
        /// キャンセル キーが入力されるまでストーリーボードを停止します。
        /// このメソッドはストーリーボードが開始している場合に限り遅延実行します。それ以外のとき、即時に実行を開始します。
        /// </summary>
        /// <param name="callback">動作が再開したときに呼び出される処理。</param>
        public void WaitForCancel(Action callback = null)
        {
            if (this.storyboard.State != StoryboardState.Started)
                this.Listen(callback, this.cancelKeys);
            else
                this.storyboard.AddAction(() => this.Listen(callback, this.cancelKeys));
        }

        /// <summary>
        /// 指定されたキーのいずれかが入力されるまでストーリーボードを停止します。
        /// このメソッドはストーリーボードが開始している場合に限り遅延実行します。それ以外のとき、即時に実行を開始します。
        /// </summary>
        /// <param name="callback">動作が再開した時に呼び出される処理。</param>
        /// <param name="keys">入力を待つキー。</param>
        public void WaitFor(Action callback = null, params Key[] keys)
        {
            if (this.storyboard.State != StoryboardState.Started)
                this.Listen(callback, this.okKeys);
            else
                this.storyboard.AddAction(() => this.Listen(callback, keys));
        }
        #endregion

        #region Private Method
        private void Listen(Action callback, Key[] keys)
        {
            this.keyUpFlag = false;
            this.storyboard.Pause();

            if (callback == null)
                this.storyboard.AddListener(() => this.ListenKeys(keys));
            else
                this.storyboard.AddListener(() => this.ListenKeysWithCallback(callback, keys));
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
                this.storyboard.Start();
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
                this.storyboard.Start();
                return true;
            }
            else
                return false;
        }

        private bool CheckAny(params Key[] keys)
        {
            for (int i = 0, j = keys.Length; i < j; i++)
                if (this.window.Keyboard[keys[i]])
                    return true;

            return false;
        }

        private bool CheckAll(params Key[] keys)
        {
            for (int i = 0, j = keys.Length; i < j; i++)
                if (!this.window.Keyboard[keys[i]])
                    return false;

            return true;
        }
        #endregion
    }
}
