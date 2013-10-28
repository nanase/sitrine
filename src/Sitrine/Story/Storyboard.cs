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
using System.Drawing;
using Sitrine.Event;
using Sitrine.Story;
using Sitrine.Utils;

namespace Sitrine
{
    /// <summary>
    /// シナリオのストーリーボードとなるイベントを格納し、遅延実行させるための仕組みを提供します。
    /// </summary>
    public abstract class Storyboard
    {
        #region -- Private Fields --
        #region Events
        private TextureEvent texture;
        private ProcessEvent process;
        private MessageEvent message;
        private KeyboardEvent keyboard;
        private MusicEvent music;
        private ScreenEvent screen;
        #endregion

        private int waitTime;
        #endregion

        #region -- Protected Fields --
        /// <summary>
        /// アクションオブジェクトを格納するリスト。この変数は読み取り専用です。
        /// </summary>
        protected readonly LinkedList<Action> Actions;

        /// <summary>
        /// リスナオブジェクトを格納するリスト。この変数は読み取り専用です。
        /// </summary>
        protected readonly List<Func<bool>> Listener;

        /// <summary>
        /// このストーリーボードが所属するウィンドウ。この変数は読み取り専用です。
        /// </summary>
        protected readonly SitrineWindow Window;
        #endregion

        #region -- Public Properties --
        #region Events
        /// <summary>
        /// テクスチャイベントを取得します。
        /// </summary>
        public TextureEvent Texture { get { return this.texture; } }

        /// <summary>
        /// プロセスイベントを取得します。
        /// </summary>
        public ProcessEvent Process { get { return this.process; } }

        /// <summary>
        /// キーボードイベントを取得します。
        /// </summary>
        public KeyboardEvent Keyboard { get { return this.keyboard; } }

        /// <summary>
        /// ミュージックイベントを取得します。
        /// </summary>
        public MusicEvent Music { get { return this.music; } }

        /// <summary>
        /// スクリーンイベントを取得します。
        /// </summary>
        public ScreenEvent Screen { get { return this.screen; } }

        /// <summary>
        /// メッセージイベントを取得します。
        /// </summary>
        public MessageEvent Message
        {
            get
            {
                if (this.message == null)
                    throw new EventNotInitalizedException("MessageEvent", "Storyboard.InitalizeMessage");
                else
                    return this.message;
            }
        }
        #endregion

        /// <summary>
        /// ストーリーボードに遅延実行を予約されたアクション数を取得します。
        /// </summary>
        public int ActionCount { get { return this.Actions.Count; } }

        /// <summary>
        /// ストーリーボードに格納されたリスナ数を取得します。
        /// </summary>
        public int ListenerCount { get { return this.Listener.Count; } }

        /// <summary>
        /// 現在のストーリーボードの状態を表す列挙体を取得します。
        /// </summary>
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

        /// <summary>
        /// ストーリーボードが 1 秒間に更新される回数を取得します。
        /// </summary>
        public virtual double UpdateFrequency
        {
            get
            {
                if (this.Window.TargetUpdateFrequency <= 0.0)
                    throw new Exception("TargetUpdateFrequency が設定されていません。GameWindow.Run メソッド呼び出し時にパラメータ 'updates_per_second' に値を指定してください。");

                return this.Window.TargetUpdateFrequency;
            }
        }

        /// <summary>
        /// ストーリーボードが終端に達したとき、自動的にストーリーボードを削除するかの真偽値を取得します。
        /// </summary>
        public bool AutoEnd { get; protected set; }
        #endregion

        #region -- Constructors --
        /// <summary>
        /// ストーリーボードが動作する SitrineWindow オブジェクトを指定して新しい Storyboard クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="Window">動作対象の SitrineWindow オブジェクト。</param>
        public Storyboard(SitrineWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            this.Actions = new LinkedList<Action>();
            this.Listener = new List<Func<bool>>();
            this.AutoEnd = true;
            this.Window = window;
            this.waitTime = 0;

            this.process = new ProcessEvent(this, this.Window);
            this.texture = new TextureEvent(this, this.Window);
            this.keyboard = new KeyboardEvent(this, this.Window);
            this.music = new MusicEvent(this, this.Window);
            this.screen = new ScreenEvent(this, this.Window);

            Trace.WriteLine("Storyboard", "Init");
        }
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// メッセージイベントを指定されたパラメータで初期化します。
        /// </summary>
        /// <param name="options">メッセージ表示に用いるテキストオプション。</param>
        /// <param name="size">メッセージ表示の描画サイズ。</param>
        public void InitalizeMessage(TextOptions options, Size size)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            if (this.message != null && this.message is IDisposable)
                ((IDisposable)this.message).Dispose();

            this.message = new MessageEvent(this, this.Window, options, size);
            Trace.WriteLine("message event", "Init");
        }

        /// <summary>
        /// 指定された秒数をこのストーリーボードでのフレーム時間に変換します。
        /// </summary>
        /// <param name="seconds">秒数。</param>
        /// <returns>フレーム時間。</returns>
        public int GetFrameCount(double seconds)
        {
            return (int)Math.Round(this.UpdateFrequency * seconds);
        }

        /// <summary>
        /// ストーリーボードの排他性をチェックします。
        /// </summary>
        /// <param name="x">IExclusiveStory を実装する 1つ目のオブジェクト。</param>
        /// <param name="y">IExclusiveStory を実装する 2つ目のオブジェクト。</param>
        /// <returns>true のとき、2つのオブジェクトはお互いに排他的であり、ストーリーボードとして共存できません。false のとき、ストーリーボードとして共存可能です。</returns>
        public static bool CheckExclusive(IExclusiveStory x, IExclusiveStory y)
        {
            return x == y || (x.TargetObject == y.TargetObject && x.TargetProperty == y.TargetProperty);
        }
        #endregion

        #region -- Internal Methods --
        internal virtual void Update()
        {
            if (this.waitTime > 0)
            {
                this.waitTime--;
            }
            else if (this.waitTime == 0)
            {
                int count = 0;

                while (this.Actions.Count > 0 && this.waitTime == 0)
                {
                    this.Actions.Last.Value();
                    this.Actions.RemoveLast();
                    count++;
                }

                DebugText.IncrementActionCount(count);
            }

            for (int i = 0; i < this.Listener.Count; i++)
            {
                if (this.Listener[i]())
                {
                    this.Listener.RemoveAt(i);
                    i--;
                }
            }

            if (this.AutoEnd && this.Actions.Count == 0 && this.Listener.Count == 0)
                this.Window.RemoveStoryboard(this);
        }

        internal void AddAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            this.Actions.AddFirst(action);
        }

        internal void AddActionNow(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            this.Actions.AddLast(action);
        }

        internal void AddListener(Func<bool> listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            this.Listener.Add(listener);
        }

        internal void SetWait(int frame)
        {
            if (frame < 0)
                throw new ArgumentOutOfRangeException("frame");

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
            this.Actions.Clear();
            this.Listener.Clear();
            this.waitTime = 0;

            this.Window.RemoveStoryboard(this);
        }
        #endregion
    }

    /// <summary>
    /// ストーリーボードの状態を表す列挙体です。
    /// </summary>
    public enum StoryboardState
    {
        /// <summary>
        /// 停止しています。ストーリーボードは再開可能で、予約されたアクションが存在しないならば再び停止します。
        /// </summary>
        Pausing,

        /// <summary>
        /// フレーム待ちです。ストーリーボードは何らかのアクションにより待ちの状態に入っています。
        /// </summary>
        Waiting,

        /// <summary>
        /// 開始しています。ストーリーボードにはアクションが存在し、フレーム待ちもありません。
        /// </summary>
        Started,
    }
}
