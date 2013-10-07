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
using System.IO;
using System.Linq;
using ux;

namespace Sitrine.Audio
{
    /// <summary>
    /// 複数のシーケンサが同時に演奏するためのレイヤを定義します。
    /// </summary>
    public class SequenceLayer
    {
        #region -- Private Field --
        private Sequencer sequencer = null;
        private readonly int[] targetParts;
        private readonly Preset preset;
        private readonly Master master;
        #endregion

        #region -- Constructor --
        /// <summary>
        /// パラメータを指定して新しい SequenceLayer クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="preset">適用されるプリセット。</param>
        /// <param name="master">シーケンサの送出先となるマスターオブジェクト。</param>
        /// <param name="targetParts">このレイヤーが通過させるハンドルのターゲットパート。</param>
        public SequenceLayer(Preset preset, Master master, IEnumerable<int> targetParts = null)
        {
            if (preset == null)
                throw new ArgumentNullException();

            if (master == null)
                throw new ArgumentNullException();

            this.preset = preset;
            this.master = master;
            this.targetParts = (targetParts == null) ? new int[0] : targetParts.ToArray();
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// SMF ファイルを読み込み、シーケンスを開始します。
        /// </summary>
        /// <param name="file">読み込まれる SMF ファイル。</param>
        public void Load(string file)
        {
            if (file == null)
                throw new ArgumentNullException();

            using (FileStream fs = new FileStream(file, FileMode.Open))
                this.Load(fs);
        }

        /// <summary>
        /// SMF データを格納したストリームを読み込み、シーケンスを開始します。
        /// </summary>
        /// <param name="stream">読み込まれるストリーム。</param>
        public void Load(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            if (!stream.CanRead)
                throw new ArgumentException();

            SmfContainer container = new SmfContainer(stream);
            HandleConverter hc = new HandleConverter(this.preset);
            hc.Convert(container);

            if (this.sequencer != null)
                this.sequencer.Stop();

            this.sequencer = new Sequencer(hc.Output, hc.Info);
            this.sequencer.Start();
            this.sequencer.OnTrackEvent += this.OnTrackEvent;
        }

        #endregion

        #region -- Private Methods --
        private void OnTrackEvent(object sender, TrackEventArgs e)
        {
            this.master.PushHandle(e.Events.Where(h => this.targetParts.Contains(h.Handle.TargetPart)).Select(i => i.Handle));
        }
        #endregion
    }
}