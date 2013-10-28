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
using System.Drawing.Text;

namespace Sitrine.Utils
{
    /// <summary>
    /// ファイルからフォントをロードするためのローダです。
    /// </summary>
    public class FontLoader : IDisposable
    {
        #region -- Private Fields --
        private readonly PrivateFontCollection fontCollection;
        private readonly FontFamily family;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// フォントから読み込みに成功したフォントファミリを取得します。
        /// </summary>
        public FontFamily Family { get { return this.family; } }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// 読み込むファイルを指定して新しい FontLoader クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="filename">読み込まれるフォントファイル。</param>
        public FontLoader(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            this.fontCollection = new PrivateFontCollection();
            this.fontCollection.AddFontFile(filename);

            if (this.fontCollection.Families.Length == 0)
                throw new Exception("指定されたファイルからフォントが見つかりません。");

            this.family = this.fontCollection.Families[0];
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.family.Dispose();
            this.fontCollection.Dispose();
        }
        #endregion
    }
}
