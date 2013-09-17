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

using Sitrine.Event;
using Sitrine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sitrine
{
    public abstract class Storyboard
    {
        #region Private Field
        private int waitTime;
        #endregion

        #region Protected Field
        protected readonly LinkedList<Action> actions;
        protected readonly List<Func<bool>> listener;
        protected readonly SitrineWindow window;
        #endregion

        #region Public Property
        public TextureEvent Texture { get; private set; }
        public ProcessEvent Process { get; private set; }
        public MessageEvent Message { get; private set; }
        public KeyboardEvent Keyboard { get; private set; }
        public int ActionCount { get { return this.actions.Count; } }
        public int ListenerCount { get { return this.listener.Count; } }

        public StoryboardState State
        {
            get
            {
                if (this.waitTime > 0)
                    return StoryboardState.Waiting;
                else if (this.waitTime == 0)
                    return StoryboardState.Started;
                else
                    return StoryboardState.Pausing;
            }
        }
        #endregion

        #region Constructor
        public Storyboard(SitrineWindow window)
        {
            this.actions = new LinkedList<Action>();
            this.listener = new List<Func<bool>>();
            this.window = window;
            this.waitTime = 0;
        }
        #endregion

        #region Public Method
        public void Update()
        {
            if (this.waitTime > 0)
            {
                this.waitTime--;
            }
            else if (this.waitTime == 0)
            {
                int count = 0;

                while (this.actions.Count > 0 && this.waitTime == 0)
                {
                    this.actions.Last.Value();
                    this.actions.RemoveLast();
                    count++;
                }

                DebugText.IncrementActionCount(count);
            }

            for (int i = 0; i < this.listener.Count; i++)
            {
                if (this.listener[i]())
                {
                    this.listener.RemoveAt(i);
                    i--;
                }
            }
        }

        public void InitalizeProcess()
        {
            if (this.Process != null && this.Process is IDisposable)
                ((IDisposable)this.Process).Dispose();

            this.Process = new ProcessEvent(this, this.window);
        }

        public void InitalizeTexture()
        {
            if (this.Texture != null && this.Texture is IDisposable)
                ((IDisposable)this.Texture).Dispose();

            this.Texture = new TextureEvent(this, this.window);
        }

        public void InitalizeMessage(TextOptions options, Size size)
        {
            if (this.Message != null && this.Message is IDisposable)
                ((IDisposable)this.Message).Dispose();

            this.Message = new MessageEvent(this, this.window, options, size);
        }

        public void InitalizeKeyboard()
        {
            if (this.Keyboard != null && this.Keyboard is IDisposable)
                ((IDisposable)this.Keyboard).Dispose();

            this.Keyboard = new KeyboardEvent(this, this.window);
        }
        #endregion

        #region Internal Method
        internal void AddAction(Action action)
        {
            this.actions.AddFirst(action);
        }

        internal void AddActionNow(Action action)
        {
            this.actions.AddLast(action);
        }

        internal void AddListener(Func<bool> listener)
        {
            this.listener.Add(listener);
        }

        internal void SetWait(int frame)
        {
            this.waitTime = frame;
        }

        internal void Pause()
        {
            this.waitTime = -1;
        }

        internal void Start()
        {
            this.waitTime = 0;
        }

        internal void Break()
        {
            this.actions.Clear();
            this.listener.Clear();
            this.waitTime = 0;
        }
        #endregion
    }

    public enum StoryboardState
    {
        Pausing,
        Waiting,
        Started,
    }
}
