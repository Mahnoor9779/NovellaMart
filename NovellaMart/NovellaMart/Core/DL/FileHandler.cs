using System.Text.Json;
using System.IO; // Explicitly adding System.IO

namespace NovellaMart.Core.DL
{
    public static class FileHandler
    {
        // CHANGED: Path now points to "Core/DL" inside your project folder.
        // Directory.GetCurrentDirectory() usually points to the project root during development.
        private static readonly string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Core", "DL");

        static FileHandler()
        {
            // Create the directory if it doesn't exist (though Core/DL should exist if this file is there)
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        // Generic Save Method
        public static void SaveData<T>(string filename, T data)
        {
            try
            {
                // Ensure filename ends with .json
                if (!filename.EndsWith(".json")) filename += ".json";

                string fullPath = Path.Combine(FolderPath, filename);

                // Options: IncludeFields is CRITICAL for your custom DSA classes (LinkedList, Node, etc.)
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true
                };

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(fullPath, jsonString);
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
                    return default(T); // Return null if file doesn't exist
                }

                string jsonString = File.ReadAllText(fullPath);

                var options = new JsonSerializerOptions
                {
                    IncludeFields = true
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