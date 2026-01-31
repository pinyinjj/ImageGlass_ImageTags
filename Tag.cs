using System.Collections.Generic;

namespace ImageTagger
{
    /// <summary>
    /// Represents a user-defined tag for grouping images.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// The unique name of the tag.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of absolute file paths belonging to this tag.
        /// </summary>
        public List<string> ImagePaths { get; set; } = new();
    }
}