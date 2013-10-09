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
using ux.Component;

namespace Sitrine.Audio
{
    class HandleConverter
    {
        #region -- Private Fields --
        private const int Part = 15 + 8;

        private readonly Preset preset;
        private readonly int[] partLsb, partMsb, partProgram;
        private readonly ProgramPreset[] nowPresets;
        private readonly int[] drumTargets = new[] { 10, 17, 18, 19, 20, 21, 22, 23 };

        private long nowTick;
        private List<SequenceItem> output;
        private SequenceInfo info;
        #endregion

        #region -- Public Properties --
        public IEnumerable<SequenceItem> Output { get { return this.output; } }

        public SequenceInfo Info { get { return this.info; } }
        #endregion

        #region -- Constructors --
        public HandleConverter(Preset preset)
        {
            this.preset = preset;

            this.partLsb = new int[HandleConverter.Part];
            this.partMsb = new int[HandleConverter.Part];
            this.partProgram = new int[HandleConverter.Part];
            this.nowPresets = new ProgramPreset[HandleConverter.Part];
        }
        #endregion

        #region -- Public Methods --
        public void Convert(SmfContainer input)
        {
            this.output = new List<SequenceItem>();
            this.info = new SequenceInfo()
            {
                Resolution = input.Resolution,
                EndOfTick = input.MaxTick,
                LoopBeginTick = this.DetectLoopBegin(input)
            };

            this.ResetHandles();

            foreach (Event @event in input.Tracks.SelectMany(t => t.Events).OrderBy(e => e.Tick))
            {
                this.nowTick = @event.Tick;

                if (@event is MidiEvent)
                    this.ConvertHandles((MidiEvent)@event);
                else if (@event is MetaEvent)
                {
                    MetaEvent metaevent = (MetaEvent)@event;
                    if (metaevent.MetaType == MetaType.Tempo)
                        this.AddTempo(metaevent.GetTempo());
                }
            }
        }
        #endregion

        #region -- Private Methods --
        private long DetectLoopBegin(SmfContainer input)
        {
            var k = input.Tracks.SelectMany(t => t.Events).OfType<MidiEvent>().Where(e => e.Type == EventType.ControlChange && e.Data1 == 111).LastOrDefault();
            return (k == null) ? 0 : k.Tick;
        }

        private void AddTempo(double tempo)
        {
            this.output.Add(new TempoItem(tempo, this.nowTick));
        }

        private void AddHandle(Handle handle)
        {
            this.output.Add(new HandleItem(handle, this.nowTick));
        }

        private void AddHandle(IEnumerable<Handle> handles)
        {
            this.output.AddRange(handles.Select(h => new HandleItem(h, this.nowTick)));
        }

        private void AddHandle(IEnumerable<Handle> handles, int targetPart)
        {
            this.output.AddRange(handles.Select(h => new HandleItem(new Handle(h, targetPart), this.nowTick)));
        }

        private void ConvertHandles(MidiEvent @event)
        {
            int part = @event.Channel + 1;

            if (part == 10)
            {
                this.ConvertDrumHandles(@event);
                return;
            }

            switch (@event.Type)
            {
                case EventType.NoteOff:
                    this.AddHandle(new Handle(part, HandleType.NoteOff, @event.Data1));
                    break;

                case EventType.NoteOn:
                    if (@event.Data2 > 0)
                        this.AddHandle(new Handle(part, HandleType.NoteOn, @event.Data1, @event.Data2 / 127f));
                    else
                        this.AddHandle(new Handle(part, HandleType.NoteOff, @event.Data1));
                    break;

                case EventType.ControlChange:
                    switch (@event.Data1)
                    {
                        case 0:
                            this.partMsb[@event.Channel] = @event.Data2;
                            break;

                        case 1:
                            if (@event.Data2 > 0)
                            {
                                this.AddHandle(new Handle(part, HandleType.Vibrate, (int)VibrateOperate.On));
                                this.AddHandle(new Handle(part, HandleType.Vibrate, (int)VibrateOperate.Depth, @event.Data2 * 0.125f));
                            }
                            else
                                this.AddHandle(new Handle(part, HandleType.Vibrate, (int)VibrateOperate.Off));
                            break;

                        case 7:
                            this.AddHandle(new Handle(part, HandleType.Volume, @event.Data2 / 127f));
                            break;

                        case 10:
                            this.AddHandle(new Handle(part, HandleType.Panpot, @event.Data2 / 64f - 1f));
                            break;

                        case 11:
                            this.AddHandle(new Handle(part, HandleType.Volume, (int)VolumeOperate.Expression, @event.Data2 / 127f));
                            break;

                        case 32:
                            this.partLsb[@event.Channel] = @event.Data2;
                            break;

                        case 120:
                            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.Silence)));
                            break;

                        case 121:
                            this.ResetHandles();
                            break;

                        case 123:
                            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.NoteOff)));
                            break;

                        default:
                            break;
                    }

                    break;

                case EventType.Pitchbend:
                    this.AddHandle(new Handle(part, HandleType.FineTune, ((((@event.Data2 << 7) | @event.Data1) - 8192) / 8192f) * 1.12246f / 8f + 1f));
                    break;

                case EventType.ProgramChange:
                    this.ChangeProgram(@event);
                    break;

                default:
                    break;
            }
        }

        private void ConvertDrumHandles(MidiEvent @event)
        {
            switch (@event.Type)
            {
                case EventType.NoteOn:

                    int target = @event.Data1 % 8;

                    if (target == 0)
                        target = 10;
                    else
                        target += 16;

                    var preset = this.preset.FindDrum(p => p.Number == @event.Data1);

                    if (preset != null)
                    {
                        this.AddHandle(preset.InitHandles, target);
                        this.AddHandle(new Handle(target, HandleType.NoteOn, preset.Note, @event.Data2));
                    }
                    break;

                case EventType.ControlChange:
                    switch (@event.Data1)
                    {
                        case 0:
                            this.partMsb[@event.Channel] = @event.Data2;
                            break;

                        case 1:
                            if (@event.Data2 > 0)
                            {
                                this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Vibrate, (int)VibrateOperate.On)));
                                this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Vibrate, (int)VibrateOperate.Depth, @event.Data2 * 0.125f)));
                            }
                            else
                                this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Vibrate, (int)VibrateOperate.Off)));
                            break;

                        case 7:
                            this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Volume, @event.Data2 / 127f)));
                            break;

                        case 10:
                            this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Panpot, @event.Data2 / 64f - 1f)));
                            break;

                        case 11:
                            this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Volume, (int)VolumeOperate.Expression, @event.Data2 / 127f)));
                            break;

                        case 32:
                            this.partLsb[@event.Channel] = @event.Data2;
                            break;

                        case 120:
                            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.Silence)));
                            break;

                        case 121:
                            this.ResetHandles();
                            break;

                        case 123:
                            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.NoteOff)));
                            break;

                        default:
                            break;
                    }

                    break;

                case EventType.Pitchbend:
                    this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.FineTune, ((((@event.Data2 << 7) | @event.Data1) - 8192) / 8192f) * 1.12246f / 8f + 1f)));
                    break;

                default:
                    break;
            }
        }

        private void ChangeProgram(MidiEvent @event)
        {
            int channel = @event.Channel;
            int part = channel + 1;

            if (part == 10)
                return;

            if (this.nowPresets[channel] != null)
                this.AddHandle(this.nowPresets[channel].FinalHandles, part);

            ProgramPreset preset = this.preset.FindProgram(p => p.Number == @event.Data1 && p.MSB == this.partMsb[channel] && p.LSB == this.partLsb[channel]) ??
                                   this.preset.FindProgram(p => p.Number == @event.Data1);

            if (preset != null)
                this.AddHandle(preset.InitHandles, part);
            else
                this.AddHandle(new Handle(part, HandleType.Waveform, (int)WaveformType.FM));

            this.nowPresets[channel] = preset;
        }

        private void ResetHandles()
        {
            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.Silence)));
            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.Reset)));
            this.AddHandle(Enumerable.Range(1, HandleConverter.Part).Select(i => new Handle(i, HandleType.Waveform, (int)WaveformType.FM)));

            this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Waveform, (int)WaveformType.LongNoise)));
            this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Envelope, (int)EnvelopeOperate.Sustain, 0.0f)));
            this.AddHandle(this.drumTargets.Select(i => new Handle(i, HandleType.Envelope, (int)EnvelopeOperate.Release, 0.0f)));
        }
        #endregion
    }
}
