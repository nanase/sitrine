using OpenTK;
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
                WindowOption option = new WindowOption()
                {
                    Title = "Sample",
                    TargetSize = new Size(320, 240),
                    WindowSize = new Size(640, 480),
                    DebugTextFontFile = "font/88 Zen.ttf",
                    DebugTextFontSize = 8,
                    TextOptions = new TextTextureOptions(new Font(font.Family, 12f, GraphicsUnit.Pixel), 17)
                };

                using (SampleWindow window = new SampleWindow(option))
                    window.Run(30.0, 60.0);
            }
        }
    }

    class SampleStory : Storyboard
    {
        public SampleStory(SitrineWindow window)
            : base(window)
        {
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
        public SampleWindow(WindowOption option)
            : base(option)
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

        protected override void OnUnload(EventArgs e)
        {
            this.textOptions.Dispose();

            base.OnUnload(e);
        }

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
