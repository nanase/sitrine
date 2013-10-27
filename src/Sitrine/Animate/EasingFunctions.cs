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

namespace Sitrine.Animate
{
    using EFunc = System.Func<double, double>;

    /// <summary>
    /// 値の連続的変化の動作を記述したイージング関数群を提供します。
    /// </summary>
    public static class EasingFunctions
    {
        #region -- Public Static Fields --
        #region Presets
        /*　Copyright 2013 jQuery Foundation and other contributors
         *  http://jquery.com/
         *
         *  Permission is hereby granted, free of charge, to any person obtaining
         *  a copy of this software and associated documentation files (the
         *  "Software"), to deal in the Software without restriction, including
         *  without limitation the rights to use, copy, modify, merge, publish,
         *  distribute, sublicense, and/or sell copies of the Software, and to
         *  permit persons to whom the Software is furnished to do so, subject to
         *  the following conditions:
         *
         *  The above copyright notice and this permission notice shall be
         *  included in all copies or substantial portions of the Software.
         *
         *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
         *  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         *  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
         *  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
         *  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
         *  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
         *  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
         */

        // refer to https://github.com/jquery/jquery/blob/2.0.3/src/effects.js#L663

        /// <summary>
        /// 直線的に変化するイージング関数。この変数は読み取り専用です。
        /// </summary>
        public static readonly EFunc Linear = x => x;
        public static readonly EFunc Swing = x => 0.5 - Math.Cos(x * Math.PI) / 2;
        #endregion

        #region Entensions
        /* Copyright 2013 jQuery Foundation and other contributors,
         * http://jqueryui.com/
         * 
         * This software consists of voluntary contributions made by many
         * individuals (AUTHORS.txt, http://jqueryui.com/about) For exact
         * contribution history, see the revision history and logs, available
         * at http://jquery-ui.googlecode.com/svn/
         * 
         * Permission is hereby granted, free of charge, to any person obtaining
         * a copy of this software and associated documentation files (the
         * "Software"), to deal in the Software without restriction, including
         * without limitation the rights to use, copy, modify, merge, publish,
         * distribute, sublicense, and/or sell copies of the Software, and to
         * permit persons to whom the Software is furnished to do so, subject to
         * the following conditions:
         * 
         * The above copyright notice and this permission notice shall be
         * included in all copies or substantial portions of the Software.
         * 
         * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
         * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
         * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
         * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
         * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
         * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
         */

        // refer to https://github.com/jquery/jquery-ui/blob/1.10.3/ui/jquery.ui.effect.js#L1236

        #region Base
        public static readonly EFunc QuadEaseIn = x => Math.Pow(x, 2);
        public static readonly EFunc CubicEaseIn = x => Math.Pow(x, 3);
        public static readonly EFunc QuartEaseIn = x => Math.Pow(x, 4);
        public static readonly EFunc QuintEaseIn = x => Math.Pow(x, 5);
        public static readonly EFunc ExpoEaseIn = x => Math.Pow(x, 6);

        public static readonly EFunc SineEaseIn = x => 1 - Math.Cos(x * Math.PI / 2);
        public static readonly EFunc CircEaseIn = x => 1 - Math.Sqrt(1 - x * x);
        public static readonly EFunc ElasticEaseIn = x => (x == 0 || x == 1) ? x : -Math.Pow(2, 8 * (x - 1)) * Math.Sin(((x - 1) * 80 - 7.5) * Math.PI / 15);
        public static readonly EFunc BackEaseIn = x => x * x * (3 * x - 2);
        public static readonly EFunc BounceEaseIn = x =>
            {
                double pow2;
                int bounce = 4;

                while (x < ((pow2 = Math.Pow(2, --bounce)) - 1) / 11) ;
                return 1 / Math.Pow(4, 3 - bounce) - 7.5625 * Math.Pow((pow2 * 3 - 2) / 22 - x, 2);
            };
        #endregion

        #region EaseOut
        public static readonly EFunc QuadEaseOut = x => 1 - QuadEaseIn(1 - x);
        public static readonly EFunc CubicEaseOut = x => 1 - CubicEaseIn(1 - x);
        public static readonly EFunc QuartEaseOut = x => 1 - QuartEaseIn(1 - x);
        public static readonly EFunc QuintEaseOut = x => 1 - QuintEaseIn(1 - x);
        public static readonly EFunc ExpoEaseOut = x => 1 - ExpoEaseIn(1 - x);
        public static readonly EFunc SineEaseOut = x => 1 - SineEaseIn(1 - x);
        public static readonly EFunc ElasticEaseOut = x => 1 - ElasticEaseIn(1 - x);
        public static readonly EFunc BackEaseOut = x => 1 - BackEaseIn(1 - x);
        public static readonly EFunc BounceEaseOut = x => 1 - BounceEaseIn(1 - x);
        #endregion

        #region EaseInOut
        public static readonly EFunc QuadEaseInOut = x => x < 0.5 ? QuadEaseIn(x * 2) / 2 : 1 - QuadEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc CubicEaseInOut = x => x < 0.5 ? CubicEaseIn(x * 2) / 2 : 1 - CubicEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc QuartEaseInOut = x => x < 0.5 ? QuartEaseIn(x * 2) / 2 : 1 - QuartEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc QuintEaseInOut = x => x < 0.5 ? QuintEaseIn(x * 2) / 2 : 1 - QuintEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc ExpoEaseInOut = x => x < 0.5 ? ExpoEaseIn(x * 2) / 2 : 1 - ExpoEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc SineEaseInOut = x => x < 0.5 ? SineEaseIn(x * 2) / 2 : 1 - SineEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc ElasticEaseInOut = x => x < 0.5 ? ElasticEaseIn(x * 2) / 2 : 1 - ElasticEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc BackEaseInOut = x => x < 0.5 ? BackEaseIn(x * 2) / 2 : 1 - BackEaseIn(x * -2 + 2) / 2;
        public static readonly EFunc BounceEaseInOut = x => x < 0.5 ? BounceEaseIn(x * 2) / 2 : 1 - BounceEaseIn(x * -2 + 2) / 2;
        #endregion
        #endregion
        #endregion
    }
}
