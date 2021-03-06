﻿/* Sitrine - 2D Game Library with OpenTK */

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
using OpenTK;
using OpenTK.Graphics;
using Sitrine.Story;
using Sitrine.Texture;
using Sitrine.Utils;

namespace Sitrine.Event
{
    /// <summary>
    /// テクスチャの管理に関わるイベントをスケジューリングします。
    /// </summary>
    public class TextureEvent : StoryEvent<TextureEvent>
    {
        #region -- Private Fields --
        private readonly Dictionary<int, int> asignment;
        #endregion

        #region -- Constructors --
        internal TextureEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.asignment = new Dictionary<int, int>();
            this.Subclass = this;
        }
        #endregion

        #region -- Public Methods --
        #region Create
        /// <summary>
        /// 指定された ID とビットマップから新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="loader">関連付けられる BitmapLoader オブジェクト。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Create(int id, BitmapLoader loader)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");

            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new BitmapTexture(loader)));
            this.Storyboard.AddResource(loader);

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
            this.AssignID = id;

            this.SetDelayToStory();
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
            this.AssignID = id;

            this.SetDelayToStory();
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
            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new BitmapTexture(loader)));
            this.Storyboard.AddResource(loader);

            return this;
        }
        #endregion

        #region CreateSpriteAnimation
        /// <summary>
        /// 指定された ID とビットマップからスプライトアニメーションとして新しいテクスチャを作成します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="loader">関連付けられる BitmapLoader オブジェクト。</param>
        /// <param name="countX">横方向への分割数。</param>
        /// <param name="countY">縦方向への分割数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent CreateSpriteAnimation(int id, BitmapLoader loader, int countX, int countY)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");

            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new SpriteAnimation(loader, countX, countY)));
            this.Storyboard.AddResource(loader);

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
            this.AssignID = id;

            this.SetDelayToStory();
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
            this.AssignID = id;

            this.SetDelayToStory();
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
            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new SpriteAnimation(loader, countX, countY)));
            this.Storyboard.AddResource(loader);

            return this;
        }
        #endregion

        #region Bind
        /// <summary>
        /// 指定された ID とビットマップを新しいテクスチャにバインドします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="loader">関連付けられる BitmapLoader オブジェクト。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Bind(int id, BitmapLoader loader)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");

            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new BitmapTexture(loader)));

            return this;
        }
        #endregion

        #region BindSpriteAnimation
        /// <summary>
        /// 指定された ID とビットマップをスプライトアニメーションとして新しいテクスチャにバインドします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">関連付けられる ID。</param>
        /// <param name="loader">関連付けられる BitmapLoader オブジェクト。</param>
        /// <param name="countX">横方向への分割数。</param>
        /// <param name="countY">縦方向への分割数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent BindSpriteAnimation(int id, BitmapLoader loader, int countX, int countY)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");

            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.AsignmentTexture(id, new SpriteAnimation(loader, countX, countY)));

            return this;
        }
        #endregion

        #region FadeIn
        /// <summary>
        /// テクスチャを指定されたフレーム時間でフェードインします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent FadeIn(DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Color(new Color4(255, 255, 255, 255), duration, easing);
        }
        #endregion

        #region FadeOut
        /// <summary>
        /// テクスチャを指定されたフレーム時間でフェードアウトします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent FadeOut(DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Color(new Color4(0, 0, 0, 0), duration, easing);
        }
        #endregion

        #region Base
        /// <summary>
        /// 拡大や回転の基点となるテクスチャ上の座標を指定します。
        /// この座標はピクセル値ではなく、テクスチャの各辺の長さを 1.0 としたときの 0.0 から 1.0 の割合で表現します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="position">基点の座標値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Base(Vector2d position)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                this.Window.Textures[this.asignment[id]].BasePoint = position;
            });

            return this;
        }

        /// <summary>
        /// 拡大や回転の基点となるテクスチャ上の座標を指定します。
        /// この座標はピクセル値ではなく、テクスチャの各辺の長さを 1.0 としたときの 0.0 から 1.0 の割合で表現します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="x">基点の X 座標値。</param>
        /// <param name="Y">基点の Y 座標値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Base(double x, double y)
        {
            return this.Base(new Vector2d(x, y));
        }
        #endregion

        #region Scale
        /// <summary>
        /// テクスチャの縦横の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scale">拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Scale(double scale)
        {
            return this.ScaleXY(scale, scale);
        }

        /// <summary>
        /// 拡大または縮小するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scale">拡大率。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Scale(double scale, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.ScaleXY(scale, scale, duration, easing);
        }

        /// <summary>
        /// テクスチャの縦横の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleX">X 軸方向の拡大率。</param>
        /// <param name="scaleY">Y 軸方向の拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleXY(double scaleX, double scaleY)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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
        /// 拡大または縮小するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleX">X 軸方向の拡大率。</param>
        /// <param name="scaleY">Y 軸方向の拡大率。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleXY(double scaleX, double scaleY, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.ScaleXY(scaleX, scaleY);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.SetDelay(delay);
                this.AnimateScale(story, this.Window.Textures[this.asignment[id]], scaleX, scaleY, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// テクスチャの X 軸方向の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleX">X 軸方向の拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleX(double scaleX)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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
        /// 拡大または縮小するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleX">X 軸方向の拡大率。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleX(double scaleX, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.ScaleX(scaleX);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                var texture = this.Window.Textures[this.asignment[id]];

                story.SetDelay(delay);
                this.AnimateScale(story, texture, scaleX, texture.ScaleY, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// テクスチャの Y 軸方向の拡大率を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleY">Y 軸方向の拡大率。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleY(double scaleY)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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
        /// 拡大または縮小するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="scaleY">Y 軸方向の拡大率。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent ScaleY(double scaleY, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.ScaleY(scaleY);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);
                var texture = this.Window.Textures[this.asignment[id]];

                story.SetDelay(delay);
                this.AnimateScale(story, texture, texture.ScaleX, scaleY, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }
        #endregion

        #region Angle
        /// <summary>
        /// テクスチャの回転角を度 (degree) で指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="angle">Z 軸方向の回転角度。単位は 度 (degree)。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Angle(double angle)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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

        /// <summary>
        /// 回転するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="angle">Z 軸方向の回転角度。単位は 度 (degree)。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Angle(double angle, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.Angle(angle);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.SetDelay(delay);
                this.AnimateAngle(story, this.Window.Textures[this.asignment[id]], angle, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }
        #endregion

        #region Position
        /// <summary>
        /// テクスチャの表示位置を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="position">表示位置。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Position(Vector2d position)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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
        /// テクスチャの表示位置を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="x">表示位置の X 座標値。</param>
        /// <param name="y">表示位置の Y 座標値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Position(double x, double y)
        {
            return this.Position(new Vector2d(x, y));
        }

        /// <summary>
        /// 指定された位置へ移動するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="position">移動先の位置座標。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Position(Vector2d position, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.Position(position);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.SetDelay(delay);
                this.AnimatePosition(story, this.Window.Textures[this.asignment[id]], position, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 指定された位置へ移動するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="x">表示位置の X 座標値。</param>
        /// <param name="y">表示位置の Y 座標値。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Position(double x, double y, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Position(new Vector2d(x, y), duration, easing);
        }
        #endregion

        #region Alpha
        /// <summary>
        /// テクスチャの透明度を変更します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="alpha">変更後の透明度。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Alpha(float alpha)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                var tex = this.Window.Textures[this.asignment[id]];
                var color = tex.Color;
                color.A = alpha;
                tex.Color = color;
            });

            return this;
        }

        /// <summary>
        /// テクスチャの透明度を変更します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="alpha">変更後の透明度。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Alpha(double alpha)
        {
            return this.Alpha((float)alpha);
        }

        /// <summary>
        /// テクスチャの透明度を変更します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="alpha">変更後の透明度。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Alpha(byte alpha)
        {
            return this.Alpha(alpha / 255.0);
        }

        /// <summary>
        /// 透明度を変更するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="alpha">変更後の透明度。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Alpha(float alpha, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.Alpha(alpha);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.SetDelay(delay);
                this.AnimateAlpha(story, this.Window.Textures[this.asignment[id]], alpha, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 透明度を変更するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="alpha">変更後の透明度。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Alpha(double alpha, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Alpha((float)alpha, duration, easing);
        }
        #endregion

        #region Color
        /// <summary>
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">反射光の色。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(Color4 color)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">反射光の色。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(Color color)
        {
            return this.Color((Color4)color);
        }

        /// <summary>
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="red">反射光の R 成分。</param>
        /// <param name="green">反射光の G 成分。</param>
        /// <param name="blue">反射光の B 成分。</param>
        /// <param name="alpha">反射光の A 成分。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            return this.Color(new Color4(red, green, blue, alpha));
        }

        /// <summary>
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="red">反射光の R 成分。</param>
        /// <param name="green">反射光の G 成分。</param>
        /// <param name="blue">反射光の B 成分。</param>
        /// <param name="alpha">反射光の A 成分。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(float red, float green, float blue, float alpha = 1.0f)
        {
            return this.Color(new Color4(red, green, blue, alpha));
        }

        /// <summary>
        /// テクスチャの反射光の色を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="red">反射光の R 成分。</param>
        /// <param name="green">反射光の G 成分。</param>
        /// <param name="blue">反射光の B 成分。</param>
        /// <param name="alpha">反射光の A 成分。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(double red, double green, double blue, double alpha = 1.0)
        {
            return this.Color(new Color4((float)red, (float)green, (float)blue, (float)alpha));
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">変化後の色。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(Color4 color, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
                return this.Color(color);

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.asignment.ContainsKey(id))
                {
                    Trace.TraceWarning("Texture ID not found: " + id);
                    return;
                }

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.SetDelay(delay);
                this.AnimateColor(story, this.Window.Textures[this.asignment[id]], color, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="color">変化後の色。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(Color color, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Color((Color4)color, duration, easing);
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="red">反射光の R 成分。</param>
        /// <param name="green">反射光の G 成分。</param>
        /// <param name="blue">反射光の B 成分。</param>
        /// <param name="alpha">反射光の A 成分。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(byte red, byte green, byte blue, byte alpha, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Color(new Color4(red, green, blue, alpha), duration, easing);
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="red">反射光の R 成分。</param>
        /// <param name="green">反射光の G 成分。</param>
        /// <param name="blue">反射光の B 成分。</param>
        /// <param name="alpha">反射光の A 成分。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(float red, float green, float blue, float alpha, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Color(new Color4(red, green, blue, alpha), duration, easing);
        }

        /// <summary>
        /// 指定された色へ変化するアニメーションを開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="red">反射光の R 成分。</param>
        /// <param name="green">反射光の G 成分。</param>
        /// <param name="blue">反射光の B 成分。</param>
        /// <param name="alpha">反射光の A 成分。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Color(double red, double green, double blue, double alpha, DelaySpan duration, Func<double, double> easing = null)
        {
            return this.Color(new Color4((float)red, (float)green, (float)blue, (float)alpha), duration, easing);
        }
        #endregion

        #region Show
        /// <summary>
        /// テクスチャを不透明にし、画面上に表示されるようにします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Show()
        {
            return this.Alpha(1.0);
        }
        #endregion

        #region Hide
        /// <summary>
        /// テクスチャを完全不透明にし、画面上から隠します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Hide()
        {
            return this.Alpha(0.0);
        }
        #endregion

        /// <summary>
        /// テクスチャのリソースを解放し画面上から消去します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Clear()
        {
            int id = this.AssignID;

            this.SetDelayToStory();
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
            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                foreach (int key in this.asignment.Values)
                    this.Window.Textures.Remove(key, true);

                this.asignment.Clear();
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
            int id = this.AssignID;

            this.SetDelayToStory();
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
        /// スプライトアニメーションのフレーム更新間隔を指定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="interval">更新フレーム間隔。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public TextureEvent Interval(int interval)
        {
            if (interval <= 0)
                throw new ArgumentOutOfRangeException("interval");

            int id = this.AssignID;

            this.SetDelayToStory();
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
        #endregion

        #region -- Private Methods --
        private void AsignmentTexture(int id, Texture.Texture texture)
        {
            if (this.asignment.ContainsKey(id))
            {
                // Re-asignment
                int key = this.asignment[id];
                this.Window.Textures.Remove(key, true);
                this.Window.Textures.Add(key, texture);
            }
            else
            {
                this.Window.Textures.Add(id, texture);
                this.asignment.Add(id, id);
            }
        }

        private void AnimateAlpha(AnimateStoryboard story, Texture.Texture texture, float alpha, DelaySpan duration, Func<double, double> easing = null)
        {
            story.TargetObject = texture;
            story.TargetProperty = "color";

            float fromAlpha = 0.0f;
            bool noCompile = false;

            float da = 0.0f;

            story.Build(duration, easing,
                () =>
                {
                    noCompile = texture.NoCompile;
                    texture.NoCompile = true;
                    fromAlpha = texture.Color.A;

                    da = (alpha - fromAlpha);
                },
                f =>
                {
                    var c = texture.Color;
                    c.A = (fromAlpha + da * f).Clamp(1.0f, 0.0f);
                    texture.Color = c;
                },
                () =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
        }

        private void AnimateAngle(AnimateStoryboard story, Texture.Texture texture, double angle, DelaySpan duration, Func<double, double> easing = null)
        {
            story.TargetObject = texture;
            story.TargetProperty = "angle";

            double fromAngle = 0.0;
            bool noCompile = false;

            double dr = 0.0;

            story.Build(duration, easing,
                () =>
                {
                    noCompile = texture.NoCompile;
                    texture.NoCompile = true;
                    fromAngle = texture.RotateZ;

                    dr = (angle - fromAngle);
                },
                f =>
                {
                    texture.RotateZ = fromAngle + dr * f;
                },
                () =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
        }

        private void AnimateScale(AnimateStoryboard story, Texture.Texture texture, double scaleX, double scaleY, DelaySpan duration, Func<double, double> easing = null)
        {
            story.TargetObject = texture;
            story.TargetProperty = "scale";

            double fromX = 0.0, fromY = 0.0;
            bool noCompile = false;

            double dx = 0.0, dy = 0.0;

            story.Build(duration, easing,
                () =>
                {
                    noCompile = texture.NoCompile;
                    texture.NoCompile = true;
                    fromX = texture.ScaleX;
                    fromY = texture.ScaleY;

                    dx = (scaleX - fromX);
                    dy = (scaleY - fromY);
                },
                f =>
                {
                    texture.ScaleX = fromX + dx * f;
                    texture.ScaleY = fromY + dy * f;
                },
                () =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
        }

        private void AnimatePosition(AnimateStoryboard story, Texture.Texture texture, Vector2d to, DelaySpan duration, Func<double, double> easing = null)
        {
            story.TargetObject = texture;
            story.TargetProperty = "position";

            Vector2d from = new Vector2d();
            bool noCompile = false;

            double dx = 0.0, dy = 0.0;

            story.Build(duration, easing,
                () =>
                {
                    noCompile = texture.NoCompile;
                    texture.NoCompile = true;
                    from = texture.Position;

                    dx = (to.X - from.X);
                    dy = (to.Y - from.Y);
                },
                f =>
                {
                    texture.Position = new Vector2d(from.X + dx * f, from.Y + dy * f);
                },
                () =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
        }

        private void AnimateColor(AnimateStoryboard story, Texture.Texture texture, Color4 to, DelaySpan duration, Func<double, double> easing = null)
        {
            story.TargetObject = texture;
            story.TargetProperty = "color";

            Color4 from = new Color4();
            bool noCompile = false;

            float dr = 0f, dg = 0f, db = 0f, da = 0f;

            story.Build(duration, easing,
                () =>
                {
                    noCompile = texture.NoCompile;
                    texture.NoCompile = true;
                    from = texture.Color;

                    dr = (to.R - from.R);
                    dg = (to.G - from.G);
                    db = (to.B - from.B);
                    da = (to.A - from.A);
                },
                f =>
                {
                    texture.Color = new Color4((from.R + dr * f).Clamp(1f, 0f),
                                               (from.G + dg * f).Clamp(1f, 0f),
                                               (from.B + db * f).Clamp(1f, 0f),
                                               (from.A + da * f).Clamp(1f, 0f));
                },
                () =>
                {
                    if (!noCompile)
                        texture.NoCompile = false;
                });
        }
        #endregion
    }
}
