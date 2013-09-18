﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sitrine;
using Sitrine.Texture;
using Sitrine.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using ux.Component;

namespace Sample
{
    static class Program
    {
        static void Main()
        {
            if (!File.Exists("message.txt"))
                return;

            using (FontLoader font = new FontLoader("font/VL-Gothic-Regular.ttf"))
            {
                TextOptions textOptions = new TextOptions(new Font(font.Family, 12f, GraphicsUnit.Pixel), 17)
                {
                    ShadowIndex = 1,
                    DrawShadow = true,
                    Antialiasing = true
                };
                textOptions.SetSolidBrushes(Color.White, Color.FromArgb(128, Color.Black), Color.OrangeRed);

                WindowOptions options = new WindowOptions()
                {
                    Title = "Sample",
                    TargetSize = new Size(320, 240),
                    WindowSize = new Size(640, 480),
                    DebugTextFontFile = "font/88 Zen.ttf",
                    DebugTextFontSize = 8,
                    TextOptions = textOptions
                };

                using (SampleWindow window = new SampleWindow(options))
                    window.Run(30.0, 60.0);
            }
        }
    }

    class SampleStory : Storyboard
    {
        public SampleStory(SitrineWindow window)
            : base(window)
        {
            this.InitalizeProcess();
            this.InitalizeMessage(window.TextOptions, new Size(320, 80));
            this.InitalizeKeyboard();
            this.InitalizeTexture();

            Message.Position = new PointF(0, 160);

            var file = File.ReadAllLines("message.txt");

            Process.Wait(0.5);
            Message.Interval = 2;
            Message.ProgressCount = 2;

            for (int i = 0; i < file.Length; i += 4)
                Message.Show(String.Join("\n", file.Skip(i).Take(4)));
        }
    }

    class SampleWindow : SitrineWindow
    {
        public SampleWindow(WindowOptions options)
            : base(options)
        {
        }

        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {
            if (this.Keyboard[OpenTK.Input.Key.Tab])
                this.debugText.IsVisible = !this.debugText.IsVisible;

            base.OnKeyPress(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.stories.Add(new SampleStory(this));

            #region Music
            this.music.Connector.Master.PushHandle(new[]{
              new Handle(1, HandleType.Envelope, (int)EnvelopeOperate.A, 0.0f),  
              new Handle(1, HandleType.Envelope, (int)EnvelopeOperate.P, 0.0f), 
              new Handle(1, HandleType.Envelope, (int)EnvelopeOperate.D, 0.025f),
              new Handle(1, HandleType.Envelope, (int)EnvelopeOperate.S, 0.0f),
              new Handle(1, HandleType.Envelope, (int)EnvelopeOperate.R, 0.0f),
              new Handle(1, HandleType.Waveform, (int)WaveformType.Square), 

              new Handle(2, HandleType.Envelope, (int)EnvelopeOperate.A, 0.0f),  
              new Handle(2, HandleType.Envelope, (int)EnvelopeOperate.P, 0.0f), 
              new Handle(2, HandleType.Envelope, (int)EnvelopeOperate.D, 0.025f),
              new Handle(2, HandleType.Envelope, (int)EnvelopeOperate.S, 0.0f),
              new Handle(2, HandleType.Envelope, (int)EnvelopeOperate.R, 0.0f),
              new Handle(2, HandleType.Waveform, (int)WaveformType.Square), 
            });
            #endregion

            base.OnLoad(e);
        }

        //protected override void OnUnload(EventArgs e)
        //{
        //    base.OnUnload(e);
        //}

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.ProcessBeforeRender(e);

            GL.PushMatrix();
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color3(Color.FromArgb(10, 59, 118));
                GL.Rect(0, 0, this.targetSize.Width, this.targetSize.Height);
                GL.Enable(EnableCap.Texture2D);
            }
            GL.PopMatrix();

            this.ProcessAfterRender(e);
        }
    }
}
