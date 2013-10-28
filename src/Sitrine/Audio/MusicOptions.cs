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

namespace Sitrine.Audio
{
    /// <summary>
    /// MusicPlayer の初期化に必要な値を格納します。
    /// </summary>
    public class MusicOptions
    {
        #region -- Private Fields --
        private int bufferSize = 1024;
        private int bufferCount = 4;
        private int samplingRate = 22050;
        private int updateInterval = 10;
        #endregion

        #region -- Public Properties --]
        /// <summary>
        /// バッファのサイズを取得または設定します。この値は偶数である必要があります。
        /// </summary>
        public int BufferSize
        {
            get
            {
                return this.bufferSize;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value % 2 == 1)
                    throw new ArgumentException("バッファサイズは偶数である必要があります。");

                this.bufferSize = value;
            }
        }

        /// <summary>
        /// バッファの保持数を取得または設定します。
        /// </summary>
        public int BufferCount
        {
            get
            {
                return this.bufferCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                this.bufferCount = value;
            }
        }

        /// <summary>
        /// シンセサイザのサンプリング周波数を取得または設定します。
        /// </summary>
        public int SamplingRate
        {
            get
            {
                return this.samplingRate;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                this.samplingRate = value;
            }
        }

        /// <summary>
        /// バッファの更新間隔を取得または設定します。値はミリ秒です。
        /// </summary>
        public int UpdateInterval
        {
            get
            {
                return this.updateInterval;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                this.updateInterval = value;
            }
        }
        #endregion
    }
}
