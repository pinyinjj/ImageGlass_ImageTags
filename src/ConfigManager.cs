using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace ImageTagger
{
    public class WindowSettings
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public FormWindowState WindowState { get; set; }
    }

    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public static WindowSettings Settings { get; private set; } = new();

        public static void Load()
        {
            if (!File.Exists(ConfigPath)) return;
            try
            {
                string json = File.ReadAllText(ConfigPath);
                Settings = JsonSerializer.Deserialize<WindowSettings>(json) ?? new();
            }
            catch 
            { 
                Settings = new();
            }
        }

        public static void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(Settings);
                File.WriteAllText(ConfigPath, json);
            }
            catch 
            { 
                // Ignore errors on save
            }
        }

        public static void SaveWindowSettings(Form form)
        {
            if (form.WindowState == FormWindowState.Normal)
            {
                Settings.Location = form.Location;
                Settings.Size = form.Size;
            }
            else if (form.WindowState != FormWindowState.Minimized)
            {
                Settings.Location = form.RestoreBounds.Location;
                Settings.Size = form.RestoreBounds.Size;
            }
            
            Settings.WindowState = form.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : form.WindowState;
            Save();
        }

        public static void ApplyWindowSettings(Form form)
        {
            Load();
            if (Settings.Size.Width > 0 && Settings.Size.Height > 0)
            {
                // Ensure the window is within at least one screen's bounds
                Rectangle windowRect = new Rectangle(Settings.Location, Settings.Size);
                bool isVisible = false;
                foreach (var screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.IntersectsWith(windowRect))
                    {
                        isVisible = true;
                        break;
                    }
                }

                if (isVisible)
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = Settings.Location;
                    form.Size = Settings.Size;
                    form.WindowState = Settings.WindowState;
                }
            }
        }
    }
}
