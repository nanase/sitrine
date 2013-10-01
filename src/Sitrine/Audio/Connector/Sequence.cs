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

/* 規格出展: about Standard MIDI format
 *          http://www2s.biglobe.ne.jp/~yyagi/material/smfspec.html
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitrine.Audio.Utils;

namespace Sitrine.Audio
{
    /// <summary>
    /// MIDI に関する一連のイベントを格納したシーケンスを提供します。
    /// </summary>
    public class Sequence
    {
        #region Private Field
        private readonly List<Track> tracks;
        #endregion

        #region Property
        /// <summary>
        /// 読み込まれた SMF のフォーマットを取得します。このプロパティは 0 または 1 のみの値となります。
        /// </summary>
        public int Format { get; private set; }

        /// <summary>
        /// シーケンスに関連付けられた分解能値を取得します。
        /// </summary>
        public int Resolution { get; private set; }

        /// <summary>
        /// 格納されたイベントの中で最大のティック数を取得します。
        /// </summary>
        public long MaxTick { get; private set; }

        /// <summary>
        /// 格納されたイベントの総数を取得します。
        /// </summary>
        public int EventCount { get; private set; }

        /// <summary>
        /// シーケンスが持つトラックの列挙子を取得します。
        /// </summary>
        public IEnumerable<Track> Tracks { get { return this.tracks; } }
        #endregion

        #region Constructor
        /// <summary>
        /// ファイルを指定して新しい Sequence クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="filename">読み込まれる SMF (MIDI) ファイル名。</param>
        public Sequence(string filename)
        {
            this.tracks = new List<Track>();
            this.LoadFile(filename);
        }
        #endregion

        #region Private Method
        private void LoadFile(string filename)
        {
            if (String.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException();

            if (!File.Exists(filename))
                throw new FileNotFoundException();

            Console.WriteLine("Loading: " + filename);

            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(stream))
            {
                // ヘッダ読み取り
                // マシンはリトルエンディアンなのでバイト列を適宜反転
                // (SMFは ビッグエンディアン で記述されている)

                // マジックナンバー: 4D 54 68 64 (MThd)
                if (br.ReadUInt32().ToLittleEndian() != 0x4d546864)
                    throw new InvalidDataException();

                // ヘッダ長 (6バイトで固定)
                if (br.ReadInt32().ToLittleEndian() != 6)
                    throw new InvalidDataException();

                // フォーマット (0 or 1. 2 についてはサポート対象外)
                this.Format = br.ReadInt16().ToLittleEndian();
                if (this.Format != 0 && this.Format != 1)
                    throw new InvalidDataException();

                // トラック数
                int trackCount = br.ReadInt16().ToLittleEndian();

                // 時間単位 (正数、つまり分解能のみサポート)
                this.Resolution = br.ReadInt16().ToLittleEndian();
                if (Resolution < 1)
                    throw new InvalidDataException();

                // トラックの追加
                int trackNumber = 0;
                while (stream.Position < stream.Length - 1)
                {
                    // マジックナンバー: 4d 54 72 6b (MTrk)
                    if (br.ReadUInt32().ToLittleEndian() == 0x4d54726b)
                    {
                        // Track クラスに処理を移す
                        this.tracks.Add(new Track(trackNumber, br));
                    }
                    else
                    {
                        // トラックチャンクでないなら、長さ分だけスキップ
                        long length = br.ReadInt32().ToLittleEndian();
                        stream.Seek(length, SeekOrigin.Current);
                    }

                    trackNumber++;
                }

                if (!this.tracks.Any(t =>
                {
                    if (!t.Events.Any())
                        return true;
                    Event e = t.Events.Last();
                    return (e.Type == EventType.MetaEvent && ((MetaEvent)e).MetaType == MetaType.EndOfTrack);
                }))
                    throw new InvalidDataException();

                this.EventCount = this.Tracks.SelectMany(t => t.Events).Count();
                this.MaxTick = this.Tracks.SelectMany(t => t.Events).Max(e => e.Tick);
            }

        }
        #endregion
    }
}
