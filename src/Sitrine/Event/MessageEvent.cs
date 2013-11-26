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
using OpenTK;
using OpenTK.Graphics;
using Sitrine.Texture;
using Sitrine.Utils;

namespace Sitrine.Event
{
    /// <summary>
    /// メッセージ表示に関するイベントをスケジューリングします。
    /// </summary>
    public class MessageEvent : StoryEvent<MessageEvent>
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
            this.Subclass = this;
        }

        internal MessageEvent(Storyboard storyboard, SitrineWindow window, TextOptions options, Size size)
            : base(storyboard, window)
        {
            this.texture = new MessageTexture(options, size);
            this.Subclass = this;
        }
        #endregion

        #region -- Public Methods --
        #region Setter
        /// <summary>
        /// 画面左上を原点にしたメッセージの座標を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent Position(Vector2d value)
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.texture.Position = value);

            return this;
        }

        /// <summary>
        /// 画面左上を原点にしたメッセージの座標を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="x">X 軸方向の座標。</param>
        /// <param name="y">Y 軸方向の座標。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent Position(double x, double y)
        {
            return this.Position(new Vector2d(x,y));
        }

        /// <summary>
        /// メッセージの反射色 (ベースとなる色) を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent Color(Color4 value)
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.texture.Color = value);

            return this;
        }

        /// <summary>
        /// メッセージの文字が更新されるフレーム間隔を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent Interval(int value)
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.texture.Interval = value);

            return this;
        }

        /// <summary>
        /// メッセージの文字が更新されるたびに描画される文字数を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="value">設定される値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent ProgressCount(int value)
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.texture.ProgressCount = value);

            return this;
        }
        #endregion

        #region Action
        /// <summary>
        /// 指定された文字列の描画を開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="text">表示されるテキスト。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MessageEvent Show(string text)
        {
            this.SetDelayToStory();
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
        #endregion
    }
}
