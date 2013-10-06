using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenTK.Input;
using Sitrine;
using Sitrine.Audio;
using Sitrine.Utils;

namespace Sample
{
    static class Program
    {
        static void Main()
        {
            if (!File.Exists("message.txt"))
                return;

            using (FontLoader font = new FontLoader("font/VL-Gothic-Regular.ttf"))
            using (FontLoader debugFont = new FontLoader("font/88 Zen.ttf"))
            {
                TextOptions textOptions = new TextOptions(new Font(font.Family, 12f, GraphicsUnit.Pixel), 17)
                {
                    ShadowIndex = 1,
                    DrawShadow = true,
                    Antialiasing = true
                };
                textOptions.SetSolidBrushes(Color.White, Color.Black, Color.OrangeRed);

                TextOptions debugTextOptions = new TextOptions(new Font(debugFont.Family, 8f, GraphicsUnit.Pixel), 8)
                {
                    ShadowIndex = 1,
                    DrawShadow = true,
                };
                debugTextOptions.SetSolidBrushes(Color.White, Color.Black);

                WindowOptions options = new WindowOptions()
                {
                    Title = "Sample",
                    TargetSize = new Size(320, 240),
                    WindowSize = new Size(640, 480),
                    DebugTextOptions = debugTextOptions,
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
            var file = File.ReadAllLines("message.txt");
            var handle = new HandleStore("sound.txt");

            this.InitalizeMessage(window.TextOptions, new Size(320, 80));

            Message.Position = new PointF(0, 160);
            Message.TextureUpdate = (s, e2) => Music.PushNow(handle["message_progress"]);

            Music.AddLayer("music", Enumerable.Range(1, 23).Except(new[] { 16 }));
            Music.Push(handle["message_init"]);

            Process.Wait(0.5);
            Message.Interval = 2;
            Message.ProgressCount = 2;

            for (int i = 0; i < file.Length; i += 4)
                Message.Show(String.Join("\n", file.Skip(i).Take(4)));
        }
    }

    class FunctionKeyStory : LoopStoryboard
    {
        public FunctionKeyStory(SitrineWindow window)
            : base(window)
        {
            Keyboard.WaitFor(window.ToggleDebugVisibility, Key.F3);
            this.StartLooping();
        }
    }

    class SampleWindow : SitrineWindow
    {
        public SampleWindow(WindowOptions options)
            : base(options)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.AddStoryboard(new SampleStory(this));
            this.AddStoryboard(new FunctionKeyStory(this));

            this.BackgroundColor = Color.FromArgb(10, 59, 118);

            base.OnLoad(e);
        }
    }
}
