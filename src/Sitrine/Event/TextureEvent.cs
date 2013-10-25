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
using Sitrine.Story;

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
        /// このメソッドは遅延実行されます。
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
        /// このメソッドは遅延実行されます。
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

        /// <summary>
        /// 指定された位置へ移動するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="to">移動先の位置座標。</param>
        /// <param name="frame">アニメーションが完了するまでのフレーム時間。</param>
        public void AnimatePosition(int id, PointF to, int frame)
        {
            if (frame == 0)
                return;

            if (frame < 0)
                throw new ArgumentOutOfRangeException("duration");

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimatePosition(this.Window.Textures[this.asignment[id]], to, frame);
                this.Window.AddStoryboard(story);
            });
        }

        /// <summary>
        /// 指定された位置へ移動するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="to">移動先の位置座標。</param>
        /// <param name="seconds">アニメーションが完了するまでの秒数。</param>
        public void AnimatePosition(int id, PointF to, double seconds)
        {
            if (seconds == 0.0)
                return;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("duration");

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimatePosition(this.Window.Textures[this.asignment[id]], to, story.GetFrameCount(seconds));
                this.Window.AddStoryboard(story);
            });
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="to">変化後の色。</param>
        /// <param name="frame">アニメーションが完了するまでのフレーム時間。</param>
        public void AnimateColor(int id, Color4 to, int frame)
        {
            if (frame == 0)
                return;

            if (frame < 0)
                throw new ArgumentOutOfRangeException("duration");

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimateColor(this.Window.Textures[this.asignment[id]], to, frame);
                this.Window.AddStoryboard(story);
            });
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられた ID。</param>
        /// <param name="to">変化後の色。</param>
        /// <param name="seconds">アニメーションが完了するまでの秒数。</param>
        public void AnimateColor(int id, Color4 to, double seconds)
        {
            if (seconds == 0.0)
                return;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("duration");

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                story.AnimateColor(this.Window.Textures[this.asignment[id]], to, story.GetFrameCount(seconds));
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

        class AnimateStoryboard : RenderStoryboard, IExclusiveStory
        {
            #region -- Private Fields --
            private object targetObject;
            private string targetPropery;
            #endregion

            #region -- Public Properties --
            public object TargetObject
            {
                get { return this.targetObject; }
            }

            public string TargetProperty
            {
                get { return this.targetPropery; }
            }
            #endregion

            #region -- Constructors --
            public AnimateStoryboard(SitrineWindow window)
                : base(window)
            {
            }
            #endregion

            #region -- Public Methods --
            public void AnimatePosition(Texture.Texture texture, PointF to, int frame)
            {
                if (frame == 0)
                    return;

                frame /= 2;

                this.targetObject = texture;
                this.targetPropery = "position";

                PointF from = new PointF();
                bool noCompile = false;

                float dx = 0f, dy = 0f;

                Process.Invoke(() =>
                {
                    texture.NoCompile = true;
                    from = texture.Position;
                    noCompile = texture.NoCompile;

                    dx = (to.X - from.X) / (float)frame;
                    dy = (to.Y - from.Y) / (float)frame;
                });

                for (int i = 0, j = 1; i < frame; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        texture.Position = new PointF(from.X + dx * j, from.Y + dy * j);
                        j++;
                    });
                }

                Process.Invoke(() =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
            }

            public void AnimateColor(Texture.Texture texture, Color4 to, int frame)
            {
                if (frame == 0)
                    return;

                frame /= 2;

                this.targetObject = texture;
                this.targetPropery = "color";

                Color4 from = new Color4();
                bool noCompile = false;

                float dr = 0f, dg = 0f, db = 0f, da = 0f;

                Process.Invoke(() =>
                {
                    texture.NoCompile = true;
                    from = texture.Color;
                    noCompile = texture.NoCompile;

                    dr = (to.R - from.R) / (float)frame;
                    dg = (to.G - from.G) / (float)frame;
                    db = (to.B - from.B) / (float)frame;
                    da = (to.A - from.A) / (float)frame;
                });

                for (int i = 0, j = 1; i < frame; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        texture.Color = new Color4(from.R + dr * j, from.G + dg * j, from.B + db * j, from.A + da * j);
                        j++;
                    });
                }

                Process.Invoke(() =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
            }
            #endregion
        }
    }
}
