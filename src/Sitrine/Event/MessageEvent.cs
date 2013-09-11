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

using OpenTK;
using OpenTK.Graphics;
using Sitrine.Texture;
using System.Drawing;
using ux.Component;

namespace Sitrine.Event
{
    public class MessageEvent : StoryEvent
    {
        #region Private Field
        private MessageTexture texture;
        #endregion

        #region Public Property
        public MessageTexture Texture { get { return this.texture; } }

        public Vector3 Position
        {
            get
            {
                return this.texture.Position;
            }
            set
            {
                this.storyboard.AddAction(() => this.texture.Position = value);
            }
        }

        public Color4 Color
        {
            get
            {
                return this.texture.Color;
            }
            set
            {
                this.storyboard.AddAction(() => this.texture.Color = value);
            }
        }

        public Color ForeColor
        {
            get
            {
                return this.texture.ForeColor;
            }
            set
            {
                this.storyboard.AddAction(() => this.texture.ForeColor = value);
            }
        }

        public int Interval
        {
            get
            {
                return this.texture.Interval;
            }
            set
            {
                this.storyboard.AddAction(() => this.texture.Interval = value);
            }
        }

        public int ProgressCount
        {
            get
            {
                return this.texture.ProgressCount;
            }
            set
            {
                this.storyboard.AddAction(() => this.texture.ProgressCount = value);
            }
        }

        public Color ShadowColor
        {
            get
            {
                return this.texture.ShadowColor;
            }
            set
            {
                this.storyboard.AddAction(() => this.texture.ShadowColor = value);
            }
        }
        #endregion

        #region Constructor
        public MessageEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.texture = new MessageTexture(this.window.TextOptions, new Size(320, 80));
            this.texture.Position = new Vector3(0, 160, 0);
        }
        #endregion

        #region Public Method
        public void Show(string text)
        {
            this.storyboard.AddAction(() =>
            {
                this.texture.Draw(text);
                this.texture.TextureUpdate += (s, e2) => this.window.Music.Connector.Master.PushHandle(new Handle(1, HandleType.NoteOn, 72, 1.0f));
                this.texture.TextureEnd += (s, e2) => this.storyboard.Keyboard.WaitForOK(() => this.window.Textures.Remove(this.texture, false));

                this.window.Textures.AddLast(this.texture);
                this.storyboard.Pause();
            });
        }
        #endregion
    }
}
