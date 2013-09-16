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

namespace Sitrine.Texture
{
    public class TextTexture : Texture
    {
        #region Private Field
        private readonly TextRender render;
        #endregion

        #region Constructor
        public TextTexture(TextRender render, Size size)
            : base(size)
        {
            this.render = render;
        }
        #endregion

        #region Public Method
        public void Draw(string text)
        {
            this.render.Clear();
            this.render.DrawString(text, 0.0f, 0.0f);
        }

        public void Draw(string text, float x, float y)
        {
            this.render.Clear();
            this.render.DrawString(text, x, y);
        }

        public override void Render()
        {
            this.render.Flush();
            Texture.Update(this.id, this.bitmap);
            base.Render();
        }

        public void Clear()
        {
            this.render.Clear();
        }

        public override void Dispose()
        {
            this.render.Dispose();
            base.Dispose();
        }
        #endregion
    }
}
