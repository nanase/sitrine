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

namespace Sitrine.Story
{
    /// <summary>
    /// 描画のためのストーリーボードです。通常のストーリーボードとは区別されます。
    /// </summary>
    public class RenderStoryboard : Storyboard
    {
        #region -- Public Properties --
        /// <summary>
        /// ストーリーボードが 1 秒間に更新される回数を取得します。
        /// </summary>
        public override double UpdateFrequency
        {
            get
            {
                if (this.Window.TargetRenderFrequency <= 0.0)
                    throw new Exception("TargetRenderFrequency が設定されていません。GameWindow.Run メソッド呼び出し時にパラメータ 'frames_per_second' に値を指定してください。");

                return this.Window.TargetRenderFrequency;
            }
        }
        #endregion

        #region -- Constructor --
        /// <summary>
        /// ストーリーボードが動作する SitrineWindow オブジェクトを指定して新しい RenderStoryboard クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="Window">動作対象の SitrineWindow オブジェクト。</param>
        public RenderStoryboard(SitrineWindow window)
            : base(window)
        {
        }
        #endregion

    }
}
