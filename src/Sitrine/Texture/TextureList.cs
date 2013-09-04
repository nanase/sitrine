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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sitrine.Texture
{
    public class TextureList : IEnumerable<Texture>, IDisposable
    {
        private readonly List<Tuple<Texture, int>> list;

        public Texture this[int index]
        {
            get { return this.list[index].Item1; }
        }

        public TextureList()
        {
            this.list = new List<Tuple<Texture, int>>();
        }

        #region IEnumerable<Texture> メンバー
        public IEnumerator<Texture> GetEnumerator()
        {
            foreach (var item in this.list)
                yield return item.Item1;
        }
        #endregion

        #region IEnumerable メンバー
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.list)
                yield return item.Item1;
        }
        #endregion

        public void Show()
        {
            for (int i = 0, j = this.list.Count; i < j; i++)
            {
                Texture tex = this.list[i].Item1;
                if (tex is IAnimationTexture)
                    ((IAnimationTexture)tex).Update();

                tex.Show();
            }
        }

        public void Add(Texture item, int zIndex)
        {
            int i = 0;
            for (int j = this.list.Count; i < j; i++)
                if (this.list[i].Item2 > zIndex)
                    break;

            this.list.Insert(i, new Tuple<Texture,int>(item, zIndex));
        }

        public void Add(Texture item)
        {
            this.list.Add(new Tuple<Texture, int>(item, this.Count));
        }

        public void Clear(bool dispose = false)
        {
            if (dispose)
            {
                foreach (var item in this.list)
                    item.Item1.Dispose();
            }

            this.list.Clear();
        }

        public bool Contains(Texture item)
        {
            return this.list.Any(t => t.Item1 == item);
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public bool Remove(Texture item, bool dispose = false)
        {
            for (int i = 0, j = this.list.Count; i < j; i++)
            {
                if (this.list[i].Item1 == item)
                {
                    this.list.RemoveAt(i);

                    if (dispose)
                        item.Dispose();

                    return true;
                }
            }

            return false;
        }

        #region IDisposable メンバー

        public void Dispose()
        {
            foreach (var item in this.list)
                item.Item1.Dispose();
        }

        #endregion
    }
}
