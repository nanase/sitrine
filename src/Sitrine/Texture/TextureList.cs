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
using System.Collections.Generic;

namespace Sitrine.Texture
{
    /// <summary>
    /// 複数のテクスチャの描画順を管理するリスト構造を提供します。
    /// </summary>
    public class TextureList : SortedList<int, Texture>, IDisposable
    {
        #region -- Public Methods --
        /// <summary>
        /// リスト上のテクスチャを必要に応じて更新し、画面上に表示させます。
        /// </summary>
        public void Render()
        {
            foreach (Texture item in this.Values)
            {
                if (item is IAnimationTexture)
                    ((IAnimationTexture)item).Update();

                item.Render();
            }
        }

        /// <summary>
        /// 指定されたテクスチャをリストの最後、つまり最背面に追加します。
        /// </summary>
        /// <param name="item">追加されるテクスチャ。</param>
        /// <returns>追加位置のキー。</returns>
        public int AddLast(Texture item)
        {
            int count = this.Count;
            int index = (count > 0) ? this.Keys[count - 1] + 1 : 0;

            this.Add(index, item);

            return index;
        }

        /// <summary>
        /// 指定されたテクスチャをリストの最初、つまり最前面に追加します。
        /// </summary>
        /// <param name="item">追加されるテクスチャ。</param>
        /// <returns>追加位置のキー。</returns>
        public int AddFirst(Texture item)
        {
            int count = this.Count;
            int index = (count > 0) ? this.Keys[0] - 1 : 0;

            this.Add(index, item);

            return index;
        }

        /// <summary>
        /// リストを空にします。
        /// </summary>
        /// <param name="dispose">オブジェクトの解放を行う場合は true、解放しない場合は false。</param>
        public void Clear(bool dispose)
        {
            if (dispose)
                foreach (var item in this.Values)
                    item.Dispose();

            base.Clear();
        }

        /// <summary>
        /// 指定されたキーのテクスチャをリストから除外します。
        /// </summary>
        /// <param name="key">テクスチャを指し示すキー。</param>
        /// <param name="dispose">テクスチャを除外と同時に解放する場合は true、解放しない場合は false。</param>
        /// <returns>除外に成功したとき true、何らかの原因で失敗したとき false。</returns>
        public bool Remove(int key, bool dispose)
        {
            if (dispose)
                this[key].Dispose();

            return this.Remove(key);
        }

        /// <summary>
        /// 指定されたテクスチャをリストから除外します。
        /// </summary>
        /// <param name="item">除外されるテクスチャ。</param>
        /// <param name="dispose">テクスチャを除外と同時に解放する場合は true、解放しない場合は false。</param>
        /// <returns>除外に成功したとき true、何らかの原因で失敗したとき false。</returns>
        public bool Remove(Texture item, bool dispose)
        {
            int index = this.IndexOfValue(item);

            if (index < 0)
                return false;

            this.RemoveAt(index);

            if (dispose)
                item.Dispose();

            return true;
        }

        /// <summary>
        /// リストを空にし、同時にすべてのテクスチャのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Clear(true);
        }
        #endregion
    }
}
