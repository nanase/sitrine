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
using System.IO;
using Sitrine.Audio;
using Sitrine.Story;
using ux.Component;

namespace Sitrine.Event
{
    /// <summary>
    /// 音楽や効果音の再生に関連するイベントをスケジューリングします。
    /// </summary>
    public class MusicEvent : StoryEvent<MusicEvent>
    {
        #region -- Constructors --
        internal MusicEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
            this.Subclass = this;
        }
        #endregion

        #region -- Public Methods --
        #region Layer
        /// <summary>
        /// シーケンスレイヤを追加します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="id">割り当てるレイヤーID。</param>
        /// <param name="targetParts">通過させるパート。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent AddLayer(int id, IEnumerable<int> targetParts = null)
        {
            this.AssignID = id;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceWarning("Add layer '{0}', but it already exists.", id);

                this.Window.Music.AddLayer(id, targetParts);
            });

            return this;
        }

        /// <summary>
        /// シーケンスレイヤを削除します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent RemoveLayer()
        {
            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceWarning("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer.Remove(id);
            });

            return this;
        }
        #endregion

        #region Load
        /// <summary>
        /// レイヤに指定された SMF ファイルをロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="file">読み込まれる SMF ファイルのパス。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Load(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");

            this.Load(file, false);

            return this;
        }

        /// <summary>
        /// レイヤに指定された SMF ファイルをロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="file">読み込まれる SMF ファイルのパス。</param>
        /// <param name="looping">ループ再生する場合は true、しない場合は false。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Load(string file, bool looping)
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");

            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].Load(file);
                this.Window.Music.Layer[id].Looping = looping;
            });

            return this;
        }

        /// <summary>
        /// レイヤに指定されたストリームからロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="stream">読み込まれるストリーム。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Load(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new NotSupportedException();

            this.Load(stream, false);

            return this;
        }

        /// <summary>
        /// レイヤに指定されたストリームからロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="stream">読み込まれるストリーム。</param>
        /// <param name="looping">ループ再生する場合は true、しない場合は false。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Load(Stream stream, bool looping)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new NotSupportedException();

            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].Load(stream);
                this.Window.Music.Layer[id].Looping = looping;
            });

            return this;
        }
        #endregion

        #region Preset
        /// <summary>
        /// プリセットをロードし、既存のプリセットと統合します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="file">読み込まれる XML ファイルのパス。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent LoadPreset(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.Window.Music.Preset.Load(file));

            return this;
        }

        /// <summary>
        /// プリセットをロードし、既存のプリセットと統合します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="stream">読み込まれるストリーム。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent LoadPreset(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new NotSupportedException();

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.Window.Music.Preset.Load(stream));

            return this;
        }

        /// <summary>
        /// プリセットをクリアします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent ClearPreset()
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(this.Window.Music.Preset.Clear);

            return this;
        }
        #endregion

        #region Control
        /// <summary>
        /// すべてのレイヤについて再生を開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent PlayAll()
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                foreach (var item in this.Window.Music.Layer.Values)
                    item.Play();
            });

            return this;
        }

        /// <summary>
        /// 指定されたレイヤについて再生を開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Play()
        {
            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].Play();
            });

            return this;
        }

        /// <summary>
        /// すべてのレイヤについて再生を停止します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent StopAll()
        {
            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                foreach (var item in this.Window.Music.Layer.Values)
                    item.Stop();
            });

            return this;
        }

        /// <summary>
        /// 指定されたレイヤについて再生を停止します。
        /// </summary>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Stop()
        {
            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].Stop();
            });

            return this;
        }
        #endregion

        #region Push
        /// <summary>
        /// ハンドル列を適用します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="handles">送信するハンドル列。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Push(IEnumerable<Handle> handles)
        {
            if (handles == null)
                throw new ArgumentNullException("handles");

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.Window.Music.Master.PushHandle(handles));

            return this;
        }

        /// <summary>
        /// ハンドル列が記述された文字列を解析し、適用します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="code">解析され送信するハンドル列。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Push(string code)
        {
            IEnumerable<Handle> handles;

            if (!HandleParser.TryParse(code, out handles))
                throw new UnableToParseHandleException();

            this.SetDelayToStory();
            this.Storyboard.AddAction(() => this.Window.Music.Master.PushHandle(handles));

            return this;
        }

        /// <summary>
        /// ハンドル列を適用します。
        /// </summary>
        /// <param name="handles">送信するハンドル列。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent PushNow(IEnumerable<Handle> handles)
        {
            if (handles == null)
                throw new ArgumentNullException("handles");

            this.SetDelayToStory();
            this.Window.Music.Master.PushHandle(handles);

            return this;
        }

        /// <summary>
        /// ハンドル列が記述された文字列を解析し、適用します。
        /// </summary>
        /// <param name="code">解析され送信するハンドル列。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent PushNow(string code)
        {
            IEnumerable<Handle> handles;

            if (!HandleParser.TryParse(code, out handles))
                throw new UnableToParseHandleException();

            this.SetDelayToStory();
            this.Window.Music.Master.PushHandle(handles);

            return this;
        }
        #endregion

        #region Loop
        /// <summary>
        /// ループの可否を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="looping">ループする場合は true、しない場合は false。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Loop(bool looping)
        {
            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].Looping = looping;
            });

            return this;
        }
        #endregion

        #region TempoFactor
        /// <summary>
        /// テンポに乗算される値を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="factor">テンポに乗算される 0.0 より大きい値。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent TempoFactor(double factor)
        {
            if (factor <= 0.0)
                throw new ArgumentOutOfRangeException("factor");

            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].TempoFactor = factor;
            });

            return this;
        }
        #endregion

        #region LoopBeginTick
        /// <summary>
        /// ループ開始ティック位置を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="factor">ループが開始されるティック位置。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent LoopBeginTick(long tick)
        {
            if (tick < 0L)
                throw new ArgumentOutOfRangeException("tick");

            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].LoopBeginTick = tick;
            });

            return this;
        }
        #endregion

        #region Tick
        /// <summary>
        /// ティック位置を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="factor">ティック位置。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Tick(long tick)
        {
            if (tick < 0L)
                throw new ArgumentOutOfRangeException("tick");

            int id = this.AssignID;

            this.SetDelayToStory();
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                this.Window.Music.Layer[id].Tick = tick;
            });

            return this;
        }
        #endregion

        #region Modurate
        /// <summary>
        /// 0.0 から 1.0 までの値を受け取るファンクションを用いて、任意のハンドルを送信します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="funcion">0.0 から 1.0 までの浮動小数点を受け取り、単一または複数のハンドルを返却するファンクション。</param>
        /// <param name="duration">アニメーションが行われる時間。</param>
        /// <param name="easing">適用するイージング関数。</param>
        /// <returns>このイベントのオブジェクトを返します。</returns>
        public MusicEvent Modurate(Func<float, IEnumerable<Handle>> funcion, DelaySpan duration, Func<double, double> easing = null)
        {
            if (duration.IsZero())
            {
                this.Window.Music.Master.PushHandle(funcion(1.0f));
                return this;
            }

            int id = this.AssignID;
            var delay = this.PopDelaySpan();

            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(id))
                    Trace.TraceError("Layer '{0}' does not exist.", id);

                AnimateStoryboard story = new AnimateStoryboard(this.Window);

                story.SetDelay(delay);
                this.AnimateModurate(story, funcion, duration, easing);
                this.Window.AddStoryboard(story);
            });

            return this;
        }
        #endregion
        #endregion

        #region -- Private Methods --
        private void AnimateModurate(AnimateStoryboard story, Func<float, IEnumerable<Handle>> funcion, DelaySpan duration, Func<double, double> easing = null)
        {
            story.TargetObject = this.Window;
            story.TargetProperty = "foreground";

            story.Build(duration, easing, null, f => this.Window.Music.Master.PushHandle(funcion(f)), null);
        }
        #endregion
    }
}
