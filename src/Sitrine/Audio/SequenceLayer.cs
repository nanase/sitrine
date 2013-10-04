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
using System.IO;
using System.Linq;
using System.Text;
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
        public SequenceLayer(Preset preset, Master master, IEnumerable<int> targetParts)
        {
            this.preset = preset;
            this.master = master;
            this.targetParts = targetParts.ToArray();
        }
        #endregion

        #region -- Public Methods --
        public void Load(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
                this.Load(fs);
        }

        public void Load(Stream stream)
        {
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
