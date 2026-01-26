using System.Collections.Generic;

namespace ImageTagger
{
    /// <summary>
    /// Represents a user-defined category for grouping images.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The unique name of the category.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of absolute file paths belonging to this category.
        /// </summary>
        public List<string> ImagePaths { get; set; } = new();
    }
}