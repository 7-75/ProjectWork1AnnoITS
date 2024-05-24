using System.Text.Json;

namespace Prove
{   
    /// <summary>
    /// Capture is a class that represents a capture
    /// </summary>
    public class Capture
    {
        public Capture(string path, string content)
        {
            Path = path;
            Content = content;
        }
        public string Content { get; private set; }
        public string Path { get; private set; }

        /// <summary>
        /// Serialize the capture to a JSON string
        /// </summary>
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}