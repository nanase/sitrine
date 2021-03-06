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
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using ux;
using ux.Utils.Midi;
using ux.Utils.Midi.Sequencer;

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

        private AudioContext context;
        private int source;
        private int[] buffers;
        private short[] sbuf;
        private float[] fbuf;

        private Master master;
        private Selector selector;
        private Preset preset;

        private Dictionary<int, SequenceLayer> layer;

        private volatile bool reqEnd;
        private Task updater;

        private bool disposed = false;
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

        public IDictionary<int, SequenceLayer> Layer { get { return this.layer; } }
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
            this.selector = new PolyphonicSelector(options.SamplingRate);
            this.master = this.selector.Master;
            this.preset = this.selector.Preset;
            this.layer = new Dictionary<int, SequenceLayer>();

            this.source = AL.GenSource();
            this.buffers = AL.GenBuffers(this.bufferCount);
            this.sbuf = new short[this.bufferSize];
            this.fbuf = new float[this.bufferSize];

            foreach (int buffer in this.buffers)
                this.FillBuffer(buffer);

            AL.SourceQueueBuffers(this.source, this.buffers.Length, this.buffers);
            AL.SourcePlay(this.source);

            this.updater = Task.Factory.StartNew(this.Update);
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// レイヤを追加します。
        /// </summary>
        /// <param name="id">キーとなる ID。</param>
        /// <param name="targetChannels">通過させる MIDI のチャネル。</param>
        public void AddLayer(int id, IEnumerable<int> targetChannels = null)
        {
            this.layer[id] = new SequenceLayer(this.preset, this.master, this.selector, targetChannels);
        }
        #endregion

        #region -- Protected Methods --
        /// <summary>
        /// このオブジェクトによって使用されているアンマネージリソースを解放し、オプションでマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は true。アンマネージリソースだけを解放する場合は false。</param>
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.updater != null)
                    {
                        this.reqEnd = true;
                        this.updater.Wait();
                        this.updater.Dispose();
                    }

                    if (this.source != -1)
                        AL.SourceStop(this.source);

                    if (this.buffers != null)
                        AL.DeleteBuffers(this.buffers);

                    if (this.source != -1)
                        AL.DeleteSource(this.source);

                    if (this.context != null)
                        this.context.Dispose();
                }

                this.updater = null;
                this.source = -1;
                this.buffers = null;
                this.context = null;

                this.sbuf = null;
                this.fbuf = null;

                this.master = null;
                this.preset = null;
                this.layer = null;

                this.disposed = true;
            }
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

        #region -- Destructors --
        ~MusicPlayer()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
