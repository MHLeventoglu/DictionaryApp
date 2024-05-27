using System.IO;

namespace PersonalDictionary;

public static class FileNameReader
{
    public static string[] GetFileNames(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            return Directory.EnumerateFiles(directoryPath)
                .Select(Path.GetFileName)
                .ToArray();
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }
    }
}