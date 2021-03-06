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

namespace Sitrine.Texture
{
    /// <summary>
    /// アニメーション動作するテクスチャが実装すべき機能を定義します。
    /// </summary>
    public interface IAnimationTexture
    {
        #region -- Properties --
        /// <summary>
        /// アニメーションが更新されるフレーム間隔を取得または設定します。
        /// </summary>
        int Interval { get; set; }
        #endregion

        #region -- Methods --
        /// <summary>
        /// テクスチャの更新を実行します。
        /// </summary>
        /// <returns>テクスチャが更新された時 true、それ以外の時 false。</returns>
        bool UpdateFrame();

        /// <summary>
        /// アニメーションを開始します。
        /// </summary>
        void Start();
        #endregion        
    }
}
