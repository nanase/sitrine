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
using ux.Component;

namespace Sitrine.Event
{
    /// <summary>
    /// 音楽や効果音の再生に関連するイベントをスケジューリングします。
    /// </summary>
    public class MusicEvent : StoryEvent
    {
        #region -- Constructors --
        internal MusicEvent(Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region -- Public Methods --
        #region Layer
        /// <summary>
        /// シーケンスレイヤを追加します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">追加されるレイヤのキーとなるレイヤ名。</param>
        /// <param name="targetParts">通過させるパート。</param>
        public void AddLayer(string key, IEnumerable<int> targetParts = null)
        {
            this.Storyboard.AddAction(() =>
            {
                if (this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceWarning("Add layer '{0}', but it already exists.", key);

                this.Window.Music.AddLayer(key, targetParts);
            });
        }

        /// <summary>
        /// シーケンスレイヤを削除します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">レイヤのキー。</param>
        public void RemoveLayer(string key)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceWarning("Layer '{0}' does not exist.", key);

                this.Window.Music.Layer.Remove(key);
            });
        }
        #endregion

        #region Load
        /// <summary>
        /// レイヤに指定された SMF ファイルをロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">レイヤのキー。</param>
        /// <param name="file">読み込まれる SMF ファイルのパス。</param>
        public void Load(string key, string file)
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentException();

            this.Load(key, file, false);
        }

        /// <summary>
        /// レイヤに指定された SMF ファイルをロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">レイヤのキー。</param>
        /// <param name="file">読み込まれる SMF ファイルのパス。</param>
        /// <param name="looping">ループ再生する場合は true、しない場合は false。</param>
        public void Load(string key, string file, bool looping)
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentException();

            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceError("Layer '{0}' does not exist.", key);

                this.Window.Music.Layer[key].Load(file);
                this.Window.Music.Layer[key].Looping = looping;
            });
        }

        /// <summary>
        /// レイヤに指定されたストリームからロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">レイヤのキー。</param>
        /// <param name="stream">読み込まれるストリーム。</param>
        public void Load(string key, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            if (!stream.CanRead)
                throw new ArgumentException();

            this.Load(key, stream, false);
        }

        /// <summary>
        /// レイヤに指定されたストリームからロードし、再生します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">レイヤのキー。</param>
        /// <param name="stream">読み込まれるストリーム。</param>
        /// <param name="looping">ループ再生する場合は true、しない場合は false。</param>
        public void Load(string key, Stream stream, bool looping)
        {
            if (stream == null)
                throw new ArgumentNullException();

            if (!stream.CanRead)
                throw new ArgumentException();

            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceError("Layer '{0}' does not exist.", key);

                this.Window.Music.Layer[key].Load(stream);
                this.Window.Music.Layer[key].Looping = looping;
            });
        }
        #endregion

        #region Preset
        /// <summary>
        /// プリセットをロードし、既存のプリセットと統合します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="file">読み込まれる XML ファイルのパス。</param>
        public void LoadPreset(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentException();

            this.Storyboard.AddAction(() => this.Window.Music.Preset.Load(file));
        }

        /// <summary>
        /// プリセットをロードし、既存のプリセットと統合します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="stream">読み込まれるストリーム。</param>
        public void LoadPreset(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            if (!stream.CanRead)
                throw new ArgumentException();

            this.Storyboard.AddAction(() => this.Window.Music.Preset.Load(stream));
        }

        /// <summary>
        /// プリセットをクリアします。
        /// このメソッドは遅延実行されます。
        /// </summary>
        public void ClearPreset()
        {
            this.Storyboard.AddAction(this.Window.Music.Preset.Clear);
        }
        #endregion

        #region Control
        /// <summary>
        /// すべてのレイヤについて再生を開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        public void Play()
        {
            this.Storyboard.AddAction(() => { foreach (var item in this.Window.Music.Layer.Values) item.Play(); });
        }

        /// <summary>
        /// 指定されたレイヤについて再生を開始します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">再生を開始するレイヤ。</param>
        public void Play(string key)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceError("Layer '{0}' does not exist.", key);

                this.Window.Music.Layer[key].Play();
            });
        }

        /// <summary>
        /// すべてのレイヤについて再生を停止します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        public void Stop()
        {
            this.Storyboard.AddAction(() => { foreach (var item in this.Window.Music.Layer.Values) item.Stop(); });
        }

        /// <summary>
        /// 指定されたレイヤについて再生を停止します。
        /// </summary>
        /// <param name="key">再生を停止するレイヤ。</param>
        public void Stop(string key)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceError("Layer '{0}' does not exist.", key);

                this.Window.Music.Layer[key].Stop();
            });
        }
        #endregion

        #region Push
        /// <summary>
        /// ハンドル列を適用します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="handles">送信するハンドル列。</param>
        public void Push(IEnumerable<Handle> handles)
        {
            if (handles == null)
                throw new ArgumentNullException();

            this.Storyboard.AddAction(() => this.Window.Music.Master.PushHandle(handles));
        }

        /// <summary>
        /// ハンドル列が記述された文字列を解析し、適用します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="code">解析され送信するハンドル列。</param>
        public void Push(string code)
        {
            IEnumerable<Handle> handles;

            if (!HandleParser.TryParse(code, out handles))
                throw new UnableToParseHandleException();

            this.Storyboard.AddAction(() => this.Window.Music.Master.PushHandle(handles));
        }

        /// <summary>
        /// ハンドル列を適用します。
        /// </summary>
        /// <param name="handles">送信するハンドル列。</param>
        public void PushNow(IEnumerable<Handle> handles)
        {
            if (handles == null)
                throw new ArgumentNullException();

            this.Window.Music.Master.PushHandle(handles);
        }

        /// <summary>
        /// ハンドル列が記述された文字列を解析し、適用します。
        /// </summary>
        /// <param name="code">解析され送信するハンドル列。</param>
        public void PushNow(string code)
        {
            IEnumerable<Handle> handles;

            if (!HandleParser.TryParse(code, out handles))
                throw new UnableToParseHandleException();

            this.Window.Music.Master.PushHandle(handles);
        }
        #endregion

        #region Setting
        /// <summary>
        /// ループの可否を設定します。
        /// このメソッドは遅延実行されます。
        /// </summary>
        /// <param name="key">設定先のレイヤのキー。</param>
        /// <param name="looping">ループする場合は true、しない場合は false。</param>
        public void SetLoop(string key, bool looping)
        {
            this.Storyboard.AddAction(() =>
            {
                if (!this.Window.Music.Layer.ContainsKey(key))
                    Trace.TraceError("Layer '{0}' does not exist.", key);

                this.Window.Music.Layer[key].Looping = looping;
            });
        }
        #endregion
        #endregion
    }
}
