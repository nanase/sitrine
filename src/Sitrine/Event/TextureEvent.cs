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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using OpenTK.Graphics;

namespace Sitrine.Event
{
    /// <summary>
    /// テクスチャの管理に関わるイベントをスケジューリングします。
    /// </summary>
    public class TextureEvent : StoryEvent
    {
        #region -- Private Fields --
        private readonly Dictionary<int, int> asignment;
        #endregion

        #region -- Constructors --
        internal TextureEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.asignment = new Dictionary<int, int>();
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 指定された ID とビットマップから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="bitmap">関連付けられる Bitmap オブジェクト。</param>
        public void Create(int id, Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException();

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new Texture.Texture(bitmap)));
        }

        /// <summary>
        /// 指定された ID とビットマップデータを格納するストリームから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="stream">ビットマップデータを格納する読み取り可能な Stream オブジェクト。</param>
        public void Create(int id, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new Texture.Texture(stream)));
        }

        /// <summary>
        /// 指定された ID とビットマップファイルから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="filename">ビットマップデータを格納したファイル名。</param>
        public void Create(int id, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException();

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new Texture.Texture(filename)));
        }

        /// <summary>
        /// 指定された ID のテクスチャを不透明にし、画面上に表示されるようにします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        public void Show(int id)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                var tex = this.Window.Textures[this.asignment[id]];
                var color = tex.Color;
                color.A = 1f;
                tex.Color = color;
            });
        }

        /// <summary>
        /// 指定された ID のテクスチャを完全不透明にし、画面上から隠します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        public void Hide(int id)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                var tex = this.Window.Textures[this.asignment[id]];
                var color = tex.Color;
                color.A = 0f;
                tex.Color = color;
            });
        }

        /// <summary>
        /// 指定された ID のテクスチャのリソースを解放し画面上から消去します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        public void Clear(int id)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures.Remove(this.asignment[id], true);
            });
        }

        /// <summary>
        /// 画面上のテクスチャをすべて解放します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        public void ClearAll()
        {
            this.Storyboard.AddAction(() =>
            {
                foreach (int key in this.asignment.Values)
                    this.Window.Textures.Remove(key, true);

                this.asignment.Clear();
            });
        }

        /// <summary>
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="color">反射光の色。</param>
        public void SetColor(int id, Color4 color)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].Color = color;
            });
        }

        /// <summary>
        /// テクスチャの表示位置を設定します。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="position">表示位置。</param>
        public void SetPosition(int id, PointF position)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].Position = position;
            });
        }

        /// <summary>
        /// テクスチャの描画処理をコンパイルしないかどうかの真偽値を設定します。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="noCompile">コンパイルしないとき true、コンパイルするとき false。</param>
        public void SetNoCompile(int id, bool noCompile)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].NoCompile = noCompile;
            });
        }

        public void AnimatePosition(int id, PointF to, int duration)
        {
            if (duration == 0)
                return;

            this.Storyboard.AddAction(() =>
            {
               if (!this.asignment.ContainsKey(id))
               {
                   Trace.TraceWarning("Texture ID not found: " + id);
                   return;
               }

               AnimateStoryboard story = new AnimateStoryboard(this.Window);
               story.AnimatePosition(this.Window.Textures[this.asignment[id]], to, duration);
               this.Window.AddStoryboard(story);
           });
        }
        #endregion

        #region -- Private Methods --
        private void AsignmentTexture(int id, Texture.Texture texture)
        {
            int key;

            if (this.asignment.ContainsKey(id))
            {
                key = this.asignment[id];
                this.Window.Textures.Remove(key, true);
                this.Window.Textures.Add(key, texture);
            }
            else
            {
                key = this.Window.Textures.AddLast(texture);
                this.asignment.Add(id, key);
            }
        }
        #endregion

        class AnimateStoryboard : Storyboard
        {
            // TODO:

            public AnimateStoryboard(SitrineWindow window)
                : base(window)
            {
            }

            public void AnimatePosition(Texture.Texture texture, PointF to, int duration)
            {
                if (duration == 0)
                    return;

                PointF from = texture.Position;
                bool noCompile = texture.NoCompile;

                float dx = (to.X - from.X) / (float)duration;
                float dy = (to.Y - from.Y) / (float)duration;

                Process.Invoke(() => texture.NoCompile = true);

                for (int i = 0, j = 0; i < duration; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        texture.Position = new PointF(from.X + dx * j, from.Y + dy * j);
                        j++;
                    });
                }

                if (!noCompile)
                    Process.Invoke(() => texture.NoCompile = false);
            }
        }
    }
}
