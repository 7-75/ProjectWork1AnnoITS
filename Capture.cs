using System.Text.Json;

namespace Prove
{
    public class Capture
    {

        public Capture(string path, string content)
        {
            Path = path;
            Content = content;
        }

        public string Content { get; private set; }
        public string Path { get; private set; }


        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}