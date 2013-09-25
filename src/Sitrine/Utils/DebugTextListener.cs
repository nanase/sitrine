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
        private readonly DateTime startupTime;
        #endregion

        #region -- Constructors --
        /// <summary>
        /// DebugTextListener クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="debugText">表示先の DebugText オブジェクト。</param>
        public DebugTextListener(DebugText debugText)
        {
            this.debugText = debugText;
            this.startupTime = DateTime.Now;
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// リスナに文字列を書き込みます。
        /// </summary>
        /// <param name="message">書き込まれる文字列。</param>
        public override void Write(string message)
        {
            this.debugText.SetDebugText(string.Format(@"[{0:hh\:mm\:ss}] {1}", DateTime.Now - this.startupTime, message));
        }

        /// <summary>
        /// リスナに文字列を書き込みます。
        /// </summary>
        /// <param name="message">書き込まれる文字列。</param>
        public override void WriteLine(string message)
        {
            this.debugText.SetDebugText(string.Format(@"[{0:hh\:mm\:ss}] {1}", DateTime.Now - this.startupTime, message));
        }

        /// <summary>
        /// トレース情報、メッセージ、およびイベント情報をリスナー固有の出力に書き込みます。
        /// </summary>
        /// <param name="eventCache">現在のプロセス ID、スレッド ID、およびスタック トレース情報を格納している System.Diagnostics.TraceEventCache オブジェクト。</param>
        /// <param name="source">出力を識別するために使用される名前。通常は、トレース イベントを生成したアプリケーションの名前。</param>
        /// <param name="eventType">トレースを発生させたイベントのタイプを指定する System.Diagnostics.TraceEventType 値の 1 つ。</param>
        /// <param name="id">イベントの数値識別子。</param>
        /// <param name="message">書き込むメッセージ。</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            string output = string.Format(@"[{0:hh\:mm\:ss}] {1}", DateTime.Now - this.startupTime, message);

            switch (eventType)
            {
                case TraceEventType.Information:
                    this.debugText.SetDebugInfoText(output);
                    break;

                case TraceEventType.Warning:
                    this.debugText.SetDebugWarningText(output);
                    break;

                case TraceEventType.Critical:
                case TraceEventType.Error:
                    this.debugText.SetDebugErrorText(output);
                    break;

                default:
                    this.debugText.SetDebugText(output);
                    break;
            }
        }

        /// <summary>
        /// トレース情報、オブジェクトの書式付き配列、およびイベント情報をリスナー固有の出力に書き込みます。
        /// </summary>
        /// <param name="eventCache">現在のプロセス ID、スレッド ID、およびスタック トレース情報を格納している System.Diagnostics.TraceEventCache オブジェクト。</param>
        /// <param name="source">出力を識別するために使用される名前。通常は、トレース イベントを生成したアプリケーションの名前。</param>
        /// <param name="eventType">トレースを発生させたイベントのタイプを指定する System.Diagnostics.TraceEventType 値の 1 つ。</param>
        /// <param name="id">イベントの数値識別子。</param>
        /// <param name="format">0 個以上の書式項目を格納している書式指定文字列。args 配列内のオブジェクトに対応します。</param>
        /// <param name="args">0 個以上の書式設定対象オブジェクトを含んだ object 配列。</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            string output = string.Format(@"[{0:hh\:mm\:ss}] ", DateTime.Now - this.startupTime) + string.Format(format, args);

            switch (eventType)
            {
                case TraceEventType.Information:
                    this.debugText.SetDebugInfoText(output);
                    break;

                case TraceEventType.Warning:
                    this.debugText.SetDebugWarningText(output);
                    break;

                case TraceEventType.Critical:
                case TraceEventType.Error:
                    this.debugText.SetDebugErrorText(output);
                    break;

                default:
                    this.debugText.SetDebugText(output);
                    break;
            }
        }
        #endregion
    }
}
