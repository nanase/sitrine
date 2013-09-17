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

using Sitrine.Utils;
using System.Drawing;

namespace Sitrine
{
    /// <summary>
    /// ウィンドウ生成に必要なオプションを格納します。
    /// </summary>
    public class WindowOptions
    {
        #region Public Property
        /// <summary>
        /// ウィンドウのサイズを取得または設定します。
        /// </summary>
        public Size WindowSize { get; set; }

        /// <summary>
        /// 描画されるサイズを取得または設定します。
        /// </summary>
        public Size TargetSize { get; set; }

        /// <summary>
        /// ウィンドウに表示されるタイトルを取得または設定します。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// デバッグ表示に使用されるフォントファイル名を取得または設定します。
        /// </summary>
        public string DebugTextFontFile { get; set; }

        /// <summary>
        /// デバッグ表示に使用されるフォントのサイズを取得または設定します。
        /// </summary>
        public int DebugTextFontSize { get; set; }

        /// <summary>
        /// テキストの表示に使用されるオプションオブジェクトを取得または設定します。このオブジェクトはデバッグ表示には使用されません。
        /// </summary>
        public TextOptions TextOptions { get; set; }
        #endregion
    }
}
