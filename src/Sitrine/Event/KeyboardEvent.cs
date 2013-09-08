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
using System.Linq;
using System.Text;

namespace Sitrine.Event
{
    public class KeyboardEvent : StoryEvent
    {
        #region Constructor
        public KeyboardEvent (Storyboard storyboard, SitrineWindow window)
            : base(storyboard, window)
        {
        }
        #endregion

        #region Public Method
        public void WaitForOK (Action callback)
        {
            throw new NotImplementedException ();
        }

        public void WaitForCancel (Action callback)
        {
            throw new NotImplementedException ();
        }

        public void WaitFor (OpenTK.Input.Key key, Action callback)
        {
            throw new NotImplementedException ();
        }

        public void WaitForEnter (Action callback)
        {
            if (callback != null)
            {
                this.storyboard.AddListener (() =>
                {
                    if (this.window.Keyboard [OpenTK.Input.Key.Enter])
                    {
                        callback ();
                        return true;
                    }
                    else
                        return false;
                }
                );
            }
            else
                this.storyboard.AddListener (() => this.window.Keyboard [OpenTK.Input.Key.Enter]);
        }
        #endregion
    }
}
