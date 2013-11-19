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
using Sitrine.Animate;
using Sitrine.Story;
using Sitrine.Texture;
using Sitrine.Utils;

namespace Sitrine.Event
{
    /// <summary>
    /// テクスチャの管理に関わるイベントをスケジューリングします。
    /// </summary>
    public class TextureEvent : AnimateEvent<TextureEvent>
    {
        #region -- Private Fields --
        private readonly Dictionary<int, int> asignment;
        private int? id = null;
        #endregion

        #region -- Constructors --
        internal TextureEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.asignment = new Dictionary<int, int>();
            this.subclass = this;
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 操作対象のテクスチャ ID を指定します。
        /// </summary>
        /// <param name="id">操作対象のテクスチャ ID。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ID(int id)
        {
            this.id = id;

            return this;
        }

        /// <summary>
        /// 指定された ID とビットマップから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="bitmap">関連付けられる Bitmap オブジェクト。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Create(int id, Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            BitmapLoader loader = new BitmapLoader(bitmap);
            this.id = id;

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new BitmapTexture(loader)));
            this.Storyboard.AddResource(loader);

            return this;
        }

        /// <summary>
        /// 指定された ID とビットマップデータを格納するストリームから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="stream">ビットマップデータを格納する読み取り可能な Stream オブジェクト。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Create(int id, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            BitmapLoader loader = new BitmapLoader(stream);
            this.id = id;

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new BitmapTexture(loader)));
            this.Storyboard.AddResource(loader);

            return this;
        }

        /// <summary>
        /// 指定された ID とビットマップファイルから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="filename">ビットマップデータを格納したファイル名。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Create(int id, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            BitmapLoader loader = new BitmapLoader(filename);
            this.id = id;

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new BitmapTexture(loader)));
            this.Storyboard.AddResource(loader);

            return this;
        }

        /// <summary>
        /// テクスチャを不透明にし、画面上に表示されるようにします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Show()
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

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

            return this;
        }

        /// <summary>
        /// テクスチャを完全不透明にし、画面上から隠します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Hide()
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

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

            return this;
        }

        /// <summary>
        /// テクスチャのリソースを解放し画面上から消去します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Clear()
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures.Remove(this.asignment[id], true);
            });

            return this;
        }

        /// <summary>
        /// 画面上のテクスチャをすべて解放します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ClearAll()
        {
            this.Storyboard.AddAction(() =>
            {
                foreach (int key in this.asignment.Values)
                    this.Window.Textures.Remove(key, true);

                this.asignment.Clear();
            });

            return this;
        }

        /// <summary>
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">反射光の色。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(Color4 color)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].Color = color;
            });

            return this;
        }

        /// <summary>
        /// テクスチャの表示位置を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="position">表示位置。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Position(PointF position)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].Position = position;
            });

            return this;
        }

        /// <summary>
        /// テクスチャの描画処理をコンパイルしないかどうかの真偽値を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="noCompile">コンパイルしないとき true、コンパイルするとき false。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent NoCompile(bool noCompile)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].NoCompile = noCompile;
            });

            return this;
        }

        /// <summary>
        /// 指定された位置へ移動するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="to">移動先の位置座標。</param>
        /// <param name="frame">アニメーションが完了するまでのフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent AnimatePosition(PointF to, int frame, Func<double, double> easing = null)
        {
            if (frame == 0)
                return this;

            if (frame < 0)
                throw new ArgumentOutOfRangeException("duration");

            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            int delayFrames = this.DelayFrames;
            double delaySeconds = this.DelaySeconds;
            this.ResetDelay();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.Process.WaitFrame(delayFrames + story.GetFrameCount(delaySeconds));

                story.AnimatePosition(this.Window.Textures[this.asignment[id]], to, frame, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 指定された位置へ移動するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="to">移動先の位置座標。</param>
        /// <param name="seconds">アニメーションが完了するまでの秒数。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent AnimatePosition(PointF to, double seconds, Func<double, double> easing = null)
        {
            if (seconds == 0.0)
                return this;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("duration");

            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            int delayFrames = this.DelayFrames;
            double delaySeconds = this.DelaySeconds;
            this.ResetDelay();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.Process.WaitFrame(delayFrames + story.GetFrameCount(delaySeconds));

                story.AnimatePosition(this.Window.Textures[this.asignment[id]], to, story.GetFrameCount(seconds), easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="to">変化後の色。</param>
        /// <param name="frame">アニメーションが完了するまでのフレーム時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent AnimateColor(Color4 to, int frame, Func<double, double> easing = null)
        {
            if (frame == 0)
                return this;

            if (frame < 0)
                throw new ArgumentOutOfRangeException("duration");

            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            int delayFrames = this.DelayFrames;
            double delaySeconds = this.DelaySeconds;
            this.ResetDelay();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.Process.WaitFrame(delayFrames + story.GetFrameCount(delaySeconds));

                story.AnimateColor(this.Window.Textures[this.asignment[id]], to, frame, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="to">変化後の色。</param>
        /// <param name="seconds">アニメーションが完了するまでの秒数。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent AnimateColor(Color4 to, double seconds, Func<double, double> easing = null)
        {
            if (seconds == 0.0)
                return this;

            if (seconds < 0.0)
                throw new ArgumentOutOfRangeException("seconds");

            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            int delayFrames = this.DelayFrames;
            double delaySeconds = this.DelaySeconds;
            this.ResetDelay();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.Process.WaitFrame(delayFrames + story.GetFrameCount(delaySeconds));

                story.AnimateColor(this.Window.Textures[this.asignment[id]], to, story.GetFrameCount(seconds), easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 指定された ID とビットマップからスプライトアニメーションとして新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="bitmap">関連付けられる Bitmap オブジェクト。</param>
        /// <param name="countX">横方向への分割数。</param>
        /// <param name="countY">縦方向への分割数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent CreateSpriteAnimation(int id, Bitmap bitmap, int countX, int countY)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            BitmapLoader loader = new BitmapLoader(bitmap);
            this.id = id;

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new SpriteAnimation(loader, countX, countY)));
            this.Storyboard.AddResource(loader);

            return this;
        }

        /// <summary>
        /// 指定された ID とビットマップデータを格納するストリームからスプライトアニメーションとして新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="stream">ビットマップデータを格納する読み取り可能な Stream オブジェクト。</param>
        /// <param name="countX">横方向への分割数。</param>
        /// <param name="countY">縦方向への分割数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent CreateSpriteAnimation(int id, Stream stream, int countX, int countY)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            BitmapLoader loader = new BitmapLoader(stream);
            this.id = id;

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new SpriteAnimation(loader, countX, countY)));
            this.Storyboard.AddResource(loader);

            return this;
        }

        /// <summary>
        /// 指定された ID とビットマップファイルからスプライトアニメーションとして新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="filename">ビットマップデータを格納したファイル名。</param>
        /// <param name="countX">横方向への分割数。</param>
        /// <param name="countY">縦方向への分割数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent CreateSpriteAnimation(int id, string filename, int countX, int countY)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            BitmapLoader loader = new BitmapLoader(filename);
            this.id = id;

            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new SpriteAnimation(loader, countX, countY)));
            this.Storyboard.AddResource(loader);

            return this;
        }

        /// <summary>
        /// スプライトアニメーションのフレーム更新間隔を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="interval">更新フレーム間隔。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Interval(int interval)
        {
            if (interval <= 0)
                throw new ArgumentOutOfRangeException("interval");

            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                Texture.Texture texture = this.Window.Textures[this.asignment[id]];

                if (texture is SpriteAnimation)
                {
                    ((SpriteAnimation)texture).Interval = interval;
                }
                else
                {
                    Trace.TraceWarning("Texture[" + id + "] is not SpriteAnimation");
                }
            });

            return this;
        }

        /// <summary>
        /// テクスチャの縦横の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scale">拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Scale(float scale)
        {
            return this.Scale(scale, scale);
        }

        /// <summary>
        /// テクスチャの縦横の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleX">X 軸方向の拡大率。</param>
        /// <param name="scaleY">Y 軸方向の拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Scale(float scaleX, float scaleY)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                var texture = this.Window.Textures[this.asignment[id]];
                texture.ScaleX = scaleX;
                texture.ScaleY = scaleY;
            });

            return this;
        }

        /// <summary>
        /// テクスチャの X 軸方向の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleX">X 軸方向の拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleX(float scaleX)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].ScaleX = scaleX;
            });

            return this;
        }

        /// <summary>
        /// テクスチャの Y 軸方向の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleY">Y 軸方向の拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleY(float scaleY)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].ScaleY = scaleY;
            });

            return this;
        }

        /// <summary>
        /// テクスチャの回転角を度 (degree) で指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="angle">Z 軸方向の回転角度。単位は 度 (degree)。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Rotate(float angle)
        {
            if (!this.id.HasValue)
                throw new KeyNotSpecifiedException();

            int id = this.id.Value;

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].RotateZ = angle;
            });

            return this;
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
            public void AnimatePosition(Texture.Texture texture, PointF to, int frame, Func<double, double> easing = null)
            {
                if (frame == 0)
                    return;

                frame /= 2;
                easing = easing ?? EasingFunctions.Linear;

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

                    dx = (to.X - from.X);
                    dy = (to.Y - from.Y);
                });

                for (int i = 0, j = 1; i < frame; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        float f = (float)easing(j++ / (double)frame);
                        texture.Position = new PointF(from.X + dx * f, from.Y + dy * f);
                    });
                }

                Process.Invoke(() =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
            }

            public void AnimateColor(Texture.Texture texture, Color4 to, int frame, Func<double, double> easing = null)
            {
                if (frame == 0)
                    return;

                frame /= 2;
                easing = easing ?? EasingFunctions.Linear;

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

                    dr = (to.R - from.R);
                    dg = (to.G - from.G);
                    db = (to.B - from.B);
                    da = (to.A - from.A);
                });

                for (int i = 0, j = 1; i < frame; i++)
                {
                    Process.WaitFrame(1);
                    Process.Invoke(() =>
                    {
                        float f = (float)easing(j++ / (double)frame);
                        texture.Color = new Color4((from.R + dr * f).Clamp(1f, 0f),
                                                   (from.G + dg * f).Clamp(1f, 0f),
                                                   (from.B + db * f).Clamp(1f, 0f),
                                                   (from.A + da * f).Clamp(1f, 0f));
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
