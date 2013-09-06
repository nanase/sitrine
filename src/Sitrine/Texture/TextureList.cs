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
    public class TextureList : SortedList<int, Texture>, IDisposable
    {
        #region Public Method
        public void Show()
        {
            foreach (Texture item in this.Values)
            {
                if (item is IAnimationTexture)
                    ((IAnimationTexture)item).Update();

                item.Show();
            }
        }

        public void AddLast(Texture item)
        {
            int count = this.Count;
            if (count > 0)
                this.Add(this.Keys[count - 1] + 1, item);
            else
                this.Add(0, item);
        }

        public void AddFirst(Texture item)
        {
            int count = this.Count;
            if (count > 0)
                this.Add(this.Keys[0] - 1, item);
            else
                this.Add(0, item);
        }

        public void Clear(bool dispose)
        {
            if (dispose)
                foreach (var item in this.Values)
                    item.Dispose();

            base.Clear();
        }

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

        public void Dispose()
        {
            this.Clear(true);
        }
        #endregion        
    }
}
