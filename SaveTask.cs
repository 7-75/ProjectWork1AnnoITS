using System;
using System.IO;
using System.Threading.Tasks;

namespace Prove;

/// <summary>
/// SaveTask is a class that represents a save task
/// </summary>
class SaveTask
{

    /// <summary>
    /// Default path for saving
    /// </summary>
    private static string DefaultPath => $"scans/{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";

    /// <summary>
    /// Save a string to a file
    /// </summary>
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

    /// <summary>
    /// Save a string to a file
    /// </summary>
    public static async Task SaveAsync(string path, Capture capture)
    {
        await SaveAsync(path, capture.Serialize());
    }

    /// <summary>
    /// Save a string to a file
    /// </summary>
    public static async Task SaveAsync(Capture capture)
    {
        await SaveAsync(DefaultPath, capture.Serialize());
    }
}

    