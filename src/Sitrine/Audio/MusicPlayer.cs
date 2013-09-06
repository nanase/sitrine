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

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Threading.Tasks;
using uxMidi;

namespace Sitrine.Audio
{
    public class MusicPlayer : IDisposable
    {
        #region Private Field
        private const int buffer_size = 1024;
        private const int buffer_count = 4;
        private const int smaplingFrequency = 22050;

        private readonly AudioContext context;
        private readonly int source;
        private readonly int[] buffers;
        private readonly short[] sbuf;
        private readonly float[] fbuf;

        private volatile bool reqEnd;
        private Task Updater;
        #endregion

        #region Public Property
        public SmfConnector Connector { get; private set; }
        #endregion

        #region Constructor
        public MusicPlayer()
        {
            this.context = new AudioContext();
            this.Connector = new SmfConnector(smaplingFrequency);

            this.source = AL.GenSource();
            this.buffers = AL.GenBuffers(buffer_count);
            this.sbuf = new short[buffer_size];
            this.fbuf = new float[buffer_size];

            foreach (int buffer in buffers)
                this.FillBuffer(buffer);

            AL.SourceQueueBuffers(source, buffers.Length, buffers);
            AL.SourcePlay(source);

            this.Updater = Task.Factory.StartNew(this.Update);
        }
        #endregion

        #region Public Method
        public void Play()
        {
            this.Connector.Play();
            this.Connector.Master.Play();
        }

        public void Stop()
        {
            this.Connector.Master.Stop();
            this.Connector.Stop();
        }

        public void Dispose()
        {
            this.reqEnd = true;
            this.Updater.Wait();
            this.Updater.Dispose();

            this.Connector.Dispose();

            AL.SourceStop(this.source);
            AL.DeleteBuffers(this.buffers);
            AL.DeleteSource(this.source);

            this.context.Dispose();
        }
        #endregion

        #region Private Method        
        private void Update()
        {
            while (!this.reqEnd)
            {
                int processed_count, queued_count;

                // 処理済みバッファの処理
                do
                {
                    AL.GetSource(source, ALGetSourcei.BuffersProcessed, out processed_count);
                    System.Threading.Thread.Sleep(10);
                }
                while (processed_count == 0);

                while (processed_count > 0)
                {
                    int buffer = AL.SourceUnqueueBuffer(source);
                    this.FillBuffer(buffer);
                    AL.SourceQueueBuffer(source, buffer);
                    --processed_count;
                }

                // キュー済みバッファの処理
                AL.GetSource(source, ALGetSourcei.BuffersQueued, out queued_count);
                if (queued_count > 0)
                {
                    int state;
                    AL.GetSource(source, ALGetSourcei.SourceState, out state);
                    if ((ALSourceState)state != ALSourceState.Playing)
                        AL.SourcePlay(source);
                }
                else
                    break;
            }
        }

        private void FillBuffer(int buffer)
        {
            this.Connector.Master.Read(this.fbuf, 0, buffer_size);

            for (int i = 0; i < buffer_size; i++)
                this.sbuf[i] = (short)(this.fbuf[i] * short.MaxValue);

            AL.BufferData(buffer, ALFormat.Stereo16, this.sbuf, sizeof(short) * buffer_size, smaplingFrequency);
        }
        #endregion
    }
}
