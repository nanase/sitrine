﻿/* Sitrine - 2D Game Library with OpenTK */

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
using System.Runtime.Serialization;

namespace Sitrine
{
    /// <summary>
    /// ストーリーイベントが初期化されないまま使用されようとしたときに発生する例外です。
    /// </summary>
    [Serializable]
    public class EventNotInitalizedException : Exception
    {
        #region -- Constructors --
        /// <summary>
        /// イベント名と初期化に必要なメソッド名を指定して新しい EventNotInitalizedException クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="initalizeMethod"></param>
        public EventNotInitalizedException(string eventName, string initalizeMethod)
            : base(string.Format("イベント {0} は初期化されていません。メソッド {1} を呼び出して初期化してください。", eventName, initalizeMethod))
        {
        }

        protected EventNotInitalizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }

    /// <summary>
    /// 文字列をハンドル列に変換しようとして解析に失敗したときに発生する例外です。
    /// </summary>
    [Serializable]
    public class UnableToParseHandleException : Exception
    {
        #region -- Constructors --
        /// <summary>
        /// 新しい UnableToParseHandleException クラスのインスタンスを初期化します。
        /// </summary>
        public UnableToParseHandleException()
            : base("ハンドル列の解析に失敗しました。")
        {
        }

        /// <summary>
        /// メッセージを指定して新しい UnableToParseHandleException クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="message">例外に加えられるメッセージ。</param>
        public UnableToParseHandleException(string message)
            : base("ハンドル列の解析に失敗しました: " + message)
        {
        }

        /// <summary>
        /// メッセージと内部例外を指定して新しい UnableToParseHandleException クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="message">例外に加えられるメッセージ。</param>
        /// <param name="inner">内部例外。</param>
        public UnableToParseHandleException(string message, Exception inner)
            : base("ハンドル列の解析に失敗しました: " + message, inner)
        {
        }

        protected UnableToParseHandleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }

    /// <summary>
    /// キーを指定すべきイベントでキーの指定がなかったときに発生する例外です。
    /// </summary>
    [Serializable]
    public class KeyNotSpecifiedException : Exception
    {
        #region -- Constructors --
        /// <summary>
        /// 新しい KeyNotSpecifiedException クラスのインスタンスを初期化します。
        /// </summary>
        public KeyNotSpecifiedException()
            : base("キーが指定されていません。")
        {
        }

        /// <summary>
        /// メッセージを指定して新しい KeyNotSpecifiedException クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="message">例外に加えられるメッセージ。</param>
        public KeyNotSpecifiedException(string message)
            : base("キーが指定されていません:" + message)
        {
        }

        /// <summary>
        /// メッセージと内部例外を指定して新しい KeyNotSpecifiedException クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="message">例外に加えられるメッセージ。</param>
        /// <param name="inner">内部例外。</param>
        public KeyNotSpecifiedException(string message, Exception inner)
            : base("キーが指定されていません:" + message, inner)
        {
        }

        protected KeyNotSpecifiedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
