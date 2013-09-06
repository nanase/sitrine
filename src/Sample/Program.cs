using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sitrine;
using Sitrine.Texture;
using Sitrine.Utils;
using System;
using System.Collections.Generic;
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

            WindowOption option = new WindowOption()
            {
                Title = "Sample",
                TargetSize = new Size(320, 240),
                WindowSize = new Size(640, 480),
                DebugTextFontFile = "font/88 Zen.ttf",
                DebugTextFontSize = 8
            };

            using (SampleWindow window = new SampleWindow(option))
                window.Run();            
        }
    }

    class SampleWindow : SitrineWindow
    {
        private readonly FontLoader font;
        private readonly TextTextureOptions textOptions;

        private string[] messages;
        private MessageTexture message;
        private MessageTexture message2;
        private bool waitKey;
        private int lineIndex = 0;

        public SampleWindow(WindowOption option)
            : base(option)
        {
            this.font = new FontLoader("font/VL-Gothic-Regular.ttf");
            this.textOptions = new TextTextureOptions(new Font(this.font.Family, 12f, GraphicsUnit.Pixel), 17);
        }

        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {
            if (this.Keyboard[OpenTK.Input.Key.Tab])
                this.debugText.ShowDebug = !this.debugText.ShowDebug;

            if (this.Keyboard[OpenTK.Input.Key.Enter] && this.waitKey)
            {
                this.message.Draw(string.Join("\n", this.messages.Skip(lineIndex).Take(4)));
                this.message.Start();

                this.message2.Draw(string.Join("\n", this.messages.Skip(lineIndex).Take(4)));
                this.message2.Start();

                this.lineIndex += 4;
                if (this.messages.Length <= this.lineIndex)
                    this.lineIndex = 0;

                this.waitKey = false;
            }

            base.OnKeyPress(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.messages = File.ReadAllLines("message.txt");

            this.message = new MessageTexture(this.textOptions, new Size(320, 80));
            this.message.Draw(@"Enter を押してください。");
            this.message.Position = new Vector3(0, 160, 0);
            this.message.Start();
            this.message.TextureUpdate += (s, e2) => this.music.Connector.Master.PushHandle(new Handle(1, HandleType.NoteOn, 72, 1.0f));
            this.message.TextureEnd += (s, e2) => this.waitKey = true;

            this.textures.AddLast(this.message);

            this.message2 = new MessageTexture(this.textOptions, new Size(320, 80));
            this.message2.Draw(@"Enter を押してください。");
            this.message2.Position = new Vector3(0, 0, 0);
            this.message2.Start();
            this.message2.TextureUpdate += (s, e2) => this.music.Connector.Master.PushHandle(new Handle(2, HandleType.NoteOn, 60, 1.0f));
            this.message2.TextureEnd += (s, e2) => this.waitKey = true;
            this.message2.ProgressCount = 1;
            this.message2.Interval = 2;
            this.message2.ForeColor = Color.Red;
            this.textures.AddLast(this.message2);

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
            this.font.Dispose();

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
