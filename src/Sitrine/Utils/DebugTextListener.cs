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

using System.Diagnostics;

namespace Sitrine.Utils
{
    /// <summary>
    /// デバッグ表示にログを出力するためのリスナです。
    /// </summary>
    class DebugTextListener : TraceListener
    {
        #region -- Private Fields --
        private readonly DebugText debugText;
        #endregion

        #region -- Constructors --
        /// <summary>
        /// DebugTextListener クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="debugText">表示先の DebugText オブジェクト。</param>
        public DebugTextListener(DebugText debugText)
        {
            this.debugText = debugText;
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// リスナに文字列を書き込みます。
        /// </summary>
        /// <param name="message">書き込まれる文字列。</param>
        public override void Write(string message)
        {
            this.debugText.SetDebugText(message);
        }

        /// <summary>
        /// リスナに文字列を書き込みます。
        /// </summary>
        /// <param name="message">書き込まれる文字列。</param>
        public override void WriteLine(string message)
        {
            this.debugText.SetDebugText(message);
        }
        #endregion
    }
}
