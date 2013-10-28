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
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using ux;

namespace Sitrine.Audio
{
    /// <summary>
    /// 外部のシンセサイザを用いて音楽を発音、演奏する機能を提供します。
    /// </summary>
    public class MusicPlayer : IDisposable
    {
        #region -- Private Fields --
        private readonly int bufferSize;
        private readonly int bufferCount;
        private readonly int samplingRate;
        private readonly int updateInterval;

        private readonly AudioContext context;
        private readonly int source;
        private readonly int[] buffers;
        private readonly short[] sbuf;
        private readonly float[] fbuf;

        private readonly Master master;
        private readonly Preset preset;

        private readonly Dictionary<string, SequenceLayer> layer;

        private volatile bool reqEnd;
        private Task Updater;
        #endregion

        #region -- Public Properties --
        /// <summary>
        /// ux のマスターオブジェクトを取得します。
        /// </summary>
        public Master Master { get { return this.master; } }

        /// <summary>
        /// 音源プリセットを取得します。
        /// </summary>
        public Preset Preset { get { return this.preset; } }

        public IDictionary<string, SequenceLayer> Layer { get { return this.layer; } }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// MusicPlayer クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MusicPlayer(MusicOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            this.bufferSize = options.BufferSize;
            this.bufferCount = options.BufferCount;
            this.samplingRate = options.SamplingRate;
            this.updateInterval = options.UpdateInterval;

            this.context = new AudioContext();
            this.master = new Master(options.SamplingRate, 23);
            this.preset = new Preset();
            this.layer = new Dictionary<string, SequenceLayer>();

            this.source = AL.GenSource();
            this.buffers = AL.GenBuffers(this.bufferCount);
            this.sbuf = new short[this.bufferSize];
            this.fbuf = new float[this.bufferSize];

            foreach (int buffer in this.buffers)
                this.FillBuffer(buffer);

            AL.SourceQueueBuffers(this.source, this.buffers.Length, this.buffers);
            AL.SourcePlay(this.source);

            this.Updater = Task.Factory.StartNew(this.Update);
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// 再生を開始します。
        /// </summary>
        public void Play()
        {
            this.master.Play();
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            this.master.Stop();
        }

        /// <summary>
        /// このオブジェクトで使われているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.reqEnd = true;
            this.Updater.Wait();
            this.Updater.Dispose();

            AL.SourceStop(this.source);
            AL.DeleteBuffers(this.buffers);
            AL.DeleteSource(this.source);

            this.context.Dispose();
        }

        /// <summary>
        /// レイヤを追加します。
        /// </summary>
        /// <param name="key">キーとなるレイヤ名。</param>
        /// <param name="tagetParts">通過させるハンドルのパート。</param>
        public void AddLayer(string key, IEnumerable<int> tagetParts = null)
        {
            this.layer[key] = new SequenceLayer(this.preset, this.master, tagetParts);
        }
        #endregion

        #region -- Private Methods --
        private void Update()
        {
            while (!this.reqEnd)
            {
                int processed_count, queued_count;

                // 処理済みバッファの処理
                do
                {
                    AL.GetSource(this.source, ALGetSourcei.BuffersProcessed, out processed_count);
                    System.Threading.Thread.Sleep(this.updateInterval);
                }
                while (processed_count == 0);

                while (processed_count > 0)
                {
                    int buffer = AL.SourceUnqueueBuffer(this.source);
                    this.FillBuffer(buffer);
                    AL.SourceQueueBuffer(this.source, buffer);
                    --processed_count;
                }

                // キュー済みバッファの処理
                AL.GetSource(this.source, ALGetSourcei.BuffersQueued, out queued_count);
                if (queued_count > 0)
                {
                    int state;
                    AL.GetSource(this.source, ALGetSourcei.SourceState, out state);
                    if ((ALSourceState)state != ALSourceState.Playing)
                        AL.SourcePlay(this.source);
                }
                else
                    break;
            }
        }

        private void FillBuffer(int buffer)
        {
            this.master.Read(this.fbuf, 0, this.bufferSize);

            for (int i = 0; i < this.bufferSize; i++)
                this.sbuf[i] = (short)(this.fbuf[i] * short.MaxValue);

            AL.BufferData(buffer, ALFormat.Stereo16, this.sbuf, sizeof(short) * this.bufferSize, this.samplingRate);
        }
        #endregion
    }
}
