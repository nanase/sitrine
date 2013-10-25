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
using OpenTK.Graphics;
using Sitrine.Texture;
using Sitrine.Utils;

namespace Sitrine.Event
{
    /// <summary>
    /// メッセージ表示に関するイベントをスケジューリングします。
    /// </summary>
    public class MessageEvent : StoryEvent
    {
        #region -- Private Fields --
        private readonly MessageTexture texture;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// メッセージ表示に関連付けられている Texture オブジェクトを取得します。
        /// </summary>
        public MessageTexture Texture { get { return this.texture; } }

        /// <summary>
        /// 画面左上を原点にしたメッセージの座標を取得または設定します。設定は遅延実行されます。
        /// </summary>
        public PointF Position
        {
            get
            {
                return this.texture.Position;
            }
            set
            {
                this.Storyboard.AddAction(() => this.texture.Position = value);
            }
        }

        /// <summary>
        /// メッセージの反射色 (ベースとなる色) を取得または設定します。設定は遅延実行されます。
        /// </summary>
        public Color4 Color
        {
            get
            {
                return this.texture.Color;
            }
            set
            {
                this.Storyboard.AddAction(() => this.texture.Color = value);
            }
        }

        /// <summary>
        /// メッセージの文字が更新されるフレーム間隔を取得または設定します。設定は遅延実行されます。
        /// </summary>
        public int Interval
        {
            get
            {
                return this.texture.Interval;
            }
            set
            {
                this.Storyboard.AddAction(() => this.texture.Interval = value);
            }
        }

        /// <summary>
        /// メッセージの文字が更新されるたびに描画される文字数を取得または設定します。設定は遅延実行されます。
        /// </summary>
        public int ProgressCount
        {
            get
            {
                return this.texture.ProgressCount;
            }
            set
            {
                this.Storyboard.AddAction(() => this.texture.ProgressCount = value);
            }
        }

        /// <summary>
        /// メッセージが更新されるたびに実行されるイベントを取得または設定します。
        /// </summary>
        public EventHandler TextureUpdate { get; set; }

        /// <summary>
        /// メッセージの更新が終了したときに実行されるイベントを取得または設定します。
        /// </summary>
        public EventHandler TextureEnd { get; set; }
        #endregion

        #region -- Constructors --
        internal MessageEvent(Storyboard storyboard, SitrineWindow window, TextRenderer renderer, Size size)
            : base(storyboard, window)
        {
            this.texture = new MessageTexture(renderer, size);
        }

        internal MessageEvent(Storyboard storyboard, SitrineWindow window, TextOptions options, Size size)
            : base(storyboard, window)
        {
            this.texture = new MessageTexture(options, size);
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された文字列の描画を開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="text">表示されるテキスト。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent Show(string text)
        {
            this.Storyboard.AddAction(() =>
            {
                this.texture.Draw(text);

                if (this.TextureUpdate != null)
                    this.texture.TextureUpdate += this.TextureUpdate;

                this.texture.TextureEnd += (s, e2) =>
                    {
                        if (this.TextureEnd != null)
                            this.TextureEnd(s, e2);

                        this.Storyboard.Keyboard.WaitForOK(() => this.Window.Textures.Remove(this.texture, false));
                    };

                this.Window.Textures.AddLast(this.texture);
                this.Storyboard.Pause();
            });

            return this;
        }
        #endregion
    }
}
