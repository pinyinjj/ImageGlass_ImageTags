using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using ImageGlass.Tools;

namespace ImageTagger
{
    /// <summary>
    /// Handles persistent storage and loading of categories and tagged image paths.
    /// </summary>
    public static class DataManager
    {
        private static readonly string StoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tags.json");
        public static List<Category> Categories { get; set; } = new();

        /// <summary>
        /// Loads category data from the local JSON file.
        /// </summary>
        public static void Load()
        {
            if (!File.Exists(StoragePath)) return;

            try
            {
                string json = File.ReadAllText(StoragePath);
                var loaded = JsonSerializer.Deserialize<List<Category>>(json);
                if (loaded == null) return;

                foreach (var category in loaded)
                {
                    var processed = new List<string>();
                    foreach (var path in category.ImagePaths)
                    {
                        if (string.IsNullOrWhiteSpace(path)) continue;

                        // Compatibility check for legacy JSON-embedded paths
                        if (path.Trim().StartsWith("{"))
                        {
                            try
                            {
                                var args = IgImageEventArgs.Deserialize(path);
                                if (!string.IsNullOrWhiteSpace(args?.FilePath)) processed.Add(args.FilePath);
                            }
                            catch { processed.Add(path); }
                        }
                        else
                        {
                            processed.Add(path);
                        }
                    }
                    category.ImagePaths = processed.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                }
                Categories = loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Storage Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves current category data to the local JSON file.
        /// </summary>
        public static void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Categories, options);
                File.WriteAllText(StoragePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Storage Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
