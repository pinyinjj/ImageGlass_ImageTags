using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq; // Added for .Any() and .Contains()

namespace ImageTagger
{
    public static class DataManager
    {
        private static readonly string _filePath;
        public static List<Category> Categories { get; set; }

        static DataManager()
        {
            // The json file will be stored in the same directory as the executable.
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tags.json");
            Categories = new List<Category>();
        }

        public static void Load()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var loadedCategories = JsonSerializer.Deserialize<List<Category>>(json);
                    if (loadedCategories != null)
                    {
                        // Process loaded paths: if they are old JSON format, extract FilePath
                        foreach (var category in loadedCategories)
                        {
                            var processedPaths = new List<string>();
                            foreach (var imagePath in category.ImagePaths)
                            {
                                if (imagePath.Trim().StartsWith("{") && imagePath.Trim().EndsWith("}"))
                                {
                                    // It's likely an old JSON format. Try to parse.
                                    try
                                    {
                                        var imageData = JsonSerializer.Deserialize<ImageGlassImageData>(imagePath);
                                        if (!string.IsNullOrWhiteSpace(imageData?.FilePath))
                                        {
                                            processedPaths.Add(imageData.FilePath);
                                        }
                                    }
                                    catch (JsonException)
                                    {
                                        // Not a valid ImageGlassImageData JSON, treat as raw path if it's not empty
                                        if (!string.IsNullOrWhiteSpace(imagePath))
                                        {
                                            processedPaths.Add(imagePath);
                                        }
                                    }
                                }
                                else
                                {
                                    // Assume it's a plain file path already
                                    if (!string.IsNullOrWhiteSpace(imagePath))
                                    {
                                        processedPaths.Add(imagePath);
                                    }
                                }
                            }
                            // Remove duplicates and assign
                            category.ImagePaths = processedPaths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                        }
                        Categories = loadedCategories;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Categories, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}