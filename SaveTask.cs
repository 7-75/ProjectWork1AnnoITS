using System;
using System.IO;
using System.Threading.Tasks;

namespace Prove;
class SaveTask
{

    private static string DefaultPath => $"scans/{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";

    public static async Task SaveAsync(string path, string content)
    {
        //create path if not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        
        await File
            .WriteAllTextAsync(path, content);
    }

    public static async Task SaveAsync(string path, Capture capture)
    {
        await SaveAsync(path, capture.Serialize());
    }

    public static async Task SaveAsync(Capture capture)
    {
        await SaveAsync(DefaultPath, capture.Serialize());
    }
}

    