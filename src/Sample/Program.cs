using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenTK.Input;
using Sitrine;
using Sitrine.Animate;
using Sitrine.Audio;
using Sitrine.Utils;

namespace Sample
{
    static class Program
    {
        static void Main()
        {
            using (FontLoader font = new FontLoader("font/VL-Gothic-Regular.ttf"))
            using (FontLoader debugFont = new FontLoader("font/88 Zen.ttf"))
            {
                TextOptions textOptions = new TextOptions(font.Family, 12, 17)
                {
                    ShadowIndex = 1,
                    DrawShadow = true,
                    Antialiasing = true
                };
                textOptions.SetSolidBrushes(Color.White, Color.Black, Color.OrangeRed);

                TextOptions debugTextOptions = new TextOptions(debugFont.Family, 8, 8)
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

                using (SitrineWindow window = new SitrineWindow(options))
                {
                    window.AddStoryboard(new SampleStory(window));
                    window.Run(30.0, 60.0);
                }
            }
        }
    }

    class SampleStory : Storyboard
    {
        public SampleStory(SitrineWindow window)
            : base(window)
        {
            #region Initalize
            var file = File.ReadAllLines("resource/message.txt");
            var handle = new HandleStore("resource/sound.txt");
            var message = this.CreateMessage(window.TextOptions, new Size(320, 80));

            message.Interval = 2;
            message.ProgressCount = 2;
            message.Position = new PointF(0, 160);
            message.TextureUpdate = (s, e2) => Music.PushNow(handle["message_progress"]);

            Screen.BackgroundColor = Color.FromArgb(10, 59, 118);
            Screen.ForegroundColor = Color.Black;
            #endregion

            Process.Loop(e => e.Keyboard.WaitFor(window.ToggleDebugVisibility, Key.F3));

            Music.Push(handle["message_init"])
                 .LoadPreset("resource/ux_preset.xml")
                 .AddLayer("music", Enumerable.Range(1, 23).Except(new[] { 16 }));

            Screen.FadeIn(5.0, EasingFunctions.QuadEaseOut);
            Process.Wait(1.0);

            for (int i = 0; i < file.Length; i += 4)
                message.Show(String.Join("\n", file.Skip(i).Take(4)));
        }
    }
}
