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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sitrine.Audio
{
    class Sequencer
    {
        #region -- Private Fields --
        private long tick;
        private double tempo = 120.0;
        private int interval = 5;
        private double tempoFactor = 1.0;
        private double tickTime;

        private int eventIndex = 0;

        private Task sequenceTask = null;
        private readonly List<HandleItem> handles;
        private readonly object syncObject = new object();
        private readonly SequenceInfo info;

        private volatile bool reqEnd;
        private volatile bool reqRewind;
        #endregion

        #region -- Public Properties --
        public double Tempo { get { return this.tempo; } }

        public long Tick
        {
            get { return this.tick; }
            set
            {
                if (value < 0)
                    throw new ArgumentException();

                this.eventIndex = 0;

                if (this.sequenceTask == null)
                {
                    this.tick = value;
                }
                else
                {
                    lock (this.syncObject)
                    {
                        this.tick = value;
                        this.reqRewind = true;
                    }
                }
            }
        }

        public int Interval
        {
            get { return this.interval; }
            set
            {
                if (value < 1)
                    throw new ArgumentException();

                this.interval = value;
            }
        }

        public double TempoFactor
        {
            get { return this.tempoFactor; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException();

                this.tempoFactor = value;
                this.RecalcTickTime();
            }
        }
        #endregion

        #region -- Public Events --
        /// <summary>
        /// シーケンサによってスケジュールされたイベントが送出される時に発生します。
        /// </summary>
        public event EventHandler<TrackEventArgs> OnTrackEvent;

        /// <summary>
        /// シーケンサが開始された時に発生します。
        /// </summary>
        public event EventHandler SequenceStarted;

        /// <summary>
        /// シーケンサが停止した時に発生します。
        /// </summary>
        public event EventHandler SequenceStopped;

        /// <summary>
        /// シーケンサがスケジュールされたイベントの最後を処理し、シーケンスの最後に達した時に発生します。
        /// </summary>
        public event EventHandler SequenceEnd;
        #endregion

        #region -- Constructors --
        public Sequencer(IEnumerable<HandleItem> handles, SequenceInfo info)
        {
            if (handles == null)
                throw new ArgumentNullException();

            this.handles = new List<HandleItem>(handles);
            this.info = info;

            this.tick = -(long)(info.Resolution * 1.0);

            this.RecalcTickTime();
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// シーケンサを開始します。
        /// </summary>
        public void Start()
        {
            if (this.sequenceTask != null && !this.sequenceTask.IsCompleted)
                return;

            if (this.SequenceStarted != null)
                this.SequenceStarted(this, new EventArgs());

            this.reqEnd = false;
            this.sequenceTask = Task.Factory.StartNew(this.Update);
        }

        /// <summary>
        /// シーケンサを停止します。
        /// </summary>
        public void Stop()
        {
            if (this.sequenceTask == null)
                return;

            if (this.SequenceStopped != null)
                this.SequenceStopped(this, new EventArgs());

            this.reqEnd = true;

            if (Task.CurrentId.HasValue && Task.CurrentId.Value == this.sequenceTask.Id)
                return;

            this.sequenceTask.Wait();
            this.sequenceTask.Dispose();
            this.sequenceTask = null;
        }
        #endregion

        #region -- Private Methods --
        private void Update()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long oldTick = 0L;
            long nowTick, startTick, endTick;
            long processTick;
            double progressTick = 0.0;

            while (!this.reqEnd)
            {
                Thread.Sleep(this.interval);

                if (this.reqEnd)
                    break;

                lock (this.syncObject)
                {
                    if (this.reqRewind)
                    {
                        nowTick = oldTick = 0L;
                        this.reqRewind = false;
                        stopwatch.Restart();
                        continue;
                    }

                    nowTick = stopwatch.ElapsedTicks;
                    progressTick += ((double)(nowTick - oldTick) * this.tickTime);

                    if (progressTick == 0.0)
                        continue;

                    processTick = (long)progressTick;
                    progressTick -= processTick;

                    startTick = this.tick;
                    endTick = startTick + processTick;

                    this.OutputEvents(this.SelectEvents(startTick, endTick).ToList());

                    oldTick = nowTick;
                    this.tick += processTick;

                    if (this.tick >= this.info.EndOfTick)
                    {
                        if (this.SequenceEnd != null)
                            this.SequenceEnd(this, new EventArgs());

                        break;
                    }
                }
            }

            this.tick = -(long)(this.info.Resolution * 1.0);

            stopwatch.Stop();
        }

        private IEnumerable<HandleItem> SelectEvents(long start, long end)
        {
            SequenceItem item;

            if (this.eventIndex < 0)
                this.eventIndex = 0;

            this.eventIndex = this.handles.FindIndex(this.eventIndex, e => e.Tick >= start);

            while (this.eventIndex >= 0 && this.eventIndex < this.handles.Count && this.handles[this.eventIndex].Tick < end)
            {
                item = this.handles[this.eventIndex++];

                if (item is TempoItem)
                    this.ChangeTempo(((TempoItem)item).Tempo);
                else
                    yield return (HandleItem)item;
            }
        }

        private void ChangeTempo(double newTempo)
        {
            if (this.tempo == newTempo)
                return;

            double oldTempo = this.tempo;
            this.tempo = newTempo;
            this.RecalcTickTime();
        }

        private void OutputEvents(IEnumerable<HandleItem> events)
        {
            if (this.OnTrackEvent != null)
                this.OnTrackEvent(this, new TrackEventArgs(events));
        }

        private void RecalcTickTime()
        {
            this.tickTime = 1.0 / ((double)Stopwatch.Frequency * ((60.0 / (this.tempo * this.tempoFactor)) / (double)this.info.Resolution));
        }
        #endregion
    }

    class TrackEventArgs : EventArgs
    {
        #region -- Public Properties --
        public IEnumerable<HandleItem> Events { get; private set; }
        #endregion

        #region -- Constructors --
        public TrackEventArgs(IEnumerable<HandleItem> events)
        {
            this.Events = events;
        }
        #endregion
    }
}
