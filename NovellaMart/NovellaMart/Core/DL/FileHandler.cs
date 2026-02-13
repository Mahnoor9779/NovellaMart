using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace NovellaMart.Core.DL
{
    public static class FileHandler
    {
        private static readonly string FolderPath = GetPathToDL();

        private static string GetPathToDL()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directoryInfo != null && directoryInfo.GetFiles("*.csproj").Length == 0)
            {
                directoryInfo = directoryInfo.Parent;
            }

            if (directoryInfo != null)
            {
                string projectRoot = directoryInfo.FullName;
                string targetPath = Path.Combine(projectRoot, "Core", "DL");

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                return targetPath;
            }

            string fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "Core", "DL");
            if (!Directory.Exists(fallbackPath)) Directory.CreateDirectory(fallbackPath);
            return fallbackPath;
        }

        public static void SaveData<T>(string filename, T data)
        {
            string logPath = Path.Combine(FolderPath, "debug_log.txt");
            try
            {
                if (!filename.EndsWith(".json")) filename += ".json";
                string fullPath = Path.Combine(FolderPath, filename);
                
                File.AppendAllText(logPath, $"[{DateTime.Now}] Attempting to save to: {fullPath}\n");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true,

                    ReferenceHandler = ReferenceHandler.IgnoreCycles,

                    MaxDepth = int.MaxValue
                };

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(fullPath, jsonString);

                string successMsg = $"[{DateTime.Now}] [DL Success] Saved to: {fullPath}\n";
                Console.WriteLine(successMsg);
                File.AppendAllText(logPath, successMsg);
            }
            catch (Exception ex)
            {
                string errorMsg = $"[{DateTime.Now}] [DL Error] Failed to save {filename}: {ex.Message}\nStack Trace: {ex.StackTrace}\n";
                Console.WriteLine(errorMsg);
                try { File.AppendAllText(logPath, errorMsg); } catch { }
            }
        }

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
                    MaxDepth = int.MaxValue
                };

                return JsonSerializer.Deserialize<T>(jsonString, options);
            }
            catch (Exception ex)
            {
                string errorMsg = $"[{DateTime.Now}] [DL Error] Failed to load {filename}: {ex.Message}\nStack Trace: {ex.StackTrace}\n";
                Console.WriteLine(errorMsg);
                try { File.AppendAllText(Path.Combine(FolderPath, "debug_log.txt"), errorMsg); } catch { }
                return default(T);
            }
        }
    }
}