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
using System.Drawing;
using System.IO;

namespace Sitrine.Event
{
    public class TextureEvent : StoryEvent
    {
        #region Private Field
        private readonly Dictionary<int, int> asignment;
        #endregion

        #region Constructor
        internal TextureEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.asignment = new Dictionary<int, int>();
        }
        #endregion

        #region Public Method
        public void Create(int id, Bitmap bitmap)
        {
            this.storyboard.AddAction(() => this.AsignmentTexture(id, new Texture.Texture(bitmap)));
        }

        public void Create(int id, Stream stream)
        {
            this.storyboard.AddAction(() => this.AsignmentTexture(id, new Texture.Texture(stream)));
        }

        public void Create(int id, string filename)
        {
            this.storyboard.AddAction(() => this.AsignmentTexture(id, new Texture.Texture(filename)));
        }

        public void Show(int id)
        {
            this.storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                    return;

                var tex = this.window.Textures[this.asignment[id]];
                var color = tex.Color;
                color.A = 1f;
                tex.Color = color;
            });
        }

        public void Hide(int id)
        {
            this.storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                    return;

                var tex = this.window.Textures[this.asignment[id]];
                var color = tex.Color;
                color.A = 0f;
                tex.Color = color;
            });
        }

        public void Clear(int id)
        {
            this.storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                    return;

                this.window.Textures.Remove(this.asignment[id], true);
            });
        }

        public void ClearAll()
        {
            this.storyboard.AddAction(() =>
            {
                foreach (int key in this.asignment.Values)
                    this.window.Textures.Remove(key, true);

                this.asignment.Clear();
            });
        }
        #endregion

        #region Private Method
        private void AsignmentTexture(int id, Texture.Texture texture)
        {
            int key;

            if (this.asignment.ContainsKey(id))
            {
                key = this.asignment[id];
                this.window.Textures.Remove(key, true);
                this.window.Textures.Add(key, texture);
            }
            else
            {
                key = this.window.Textures.AddLast(texture);
                this.asignment.Add(id, key);
            }
        }
        #endregion
    }
}
