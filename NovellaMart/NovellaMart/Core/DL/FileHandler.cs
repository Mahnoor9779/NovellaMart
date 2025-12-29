using System.Text.Json;
using System.Text.Json.Serialization; // Required for ReferenceHandler
using System.IO;

namespace NovellaMart.Core.DL
{
    public static class FileHandler
    {
        private static readonly string FolderPath = GetPathToDL();

        // Helper to find the actual "Core/DL" folder in your source code
        private static string GetPathToDL()
        {
            // Start from the execution directory (usually bin/Debug/net8.0/...)
            DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            // Traverse up until we find the .csproj file, which indicates the Project Root
            while (directoryInfo != null && directoryInfo.GetFiles("*.csproj").Length == 0)
            {
                directoryInfo = directoryInfo.Parent;
            }

            // If we found the project root, force the path to be Core/DL inside it
            if (directoryInfo != null)
            {
                string projectRoot = directoryInfo.FullName;
                string targetPath = Path.Combine(projectRoot, "Core", "DL");

                // Create it if it doesn't exist yet
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                return targetPath;
            }

            // Fallback: If not found, use current execution directory
            string fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "Core", "DL");
            if (!Directory.Exists(fallbackPath)) Directory.CreateDirectory(fallbackPath);
            return fallbackPath;
        }

        // Generic Save Method
        public static void SaveData<T>(string filename, T data)
        {
            try
            {
                if (!filename.EndsWith(".json")) filename += ".json";
                string fullPath = Path.Combine(FolderPath, filename);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true, // Serialize public fields (like 'head')

                    // FIX 1: Handle Circular References (A -> B -> A)
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,

                    // FIX 2: Increase Depth Limit for Linked Lists (head.next.next...)
                    MaxDepth = int.MaxValue
                };

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(fullPath, jsonString);

                Console.WriteLine($"[DL Success] Saved to: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DL Error] Failed to save {filename}: {ex.Message}");
            }
        }

        // Generic Load Method
        public static T LoadData<T>(string filename)
        {
            try
            {
                if (!filename.EndsWith(".json")) filename += ".json";
                string fullPath = Path.Combine(FolderPath, filename);

                if (!File.Exists(fullPath))
                {
                    return default(T);
                }

                string jsonString = File.ReadAllText(fullPath);

                var options = new JsonSerializerOptions
                {
                    IncludeFields = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    MaxDepth = int.MaxValue // Critical for deep structures
                };

                return JsonSerializer.Deserialize<T>(jsonString, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DL Error] Failed to load {filename}: {ex.Message}");
                return default(T);
            }
        }
    }
}