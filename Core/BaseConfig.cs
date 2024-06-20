using System.Text.Json;

namespace TBAntiCheat.Core
{
    internal class BaseConfig
    {
        internal static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
    }

    internal class BaseConfig<T> where T : new()
    {
        internal T Config { get; private set; }
        protected readonly string configPath = string.Empty;

        internal BaseConfig(string path)
        {
            string folderPath = $"{Globals.GetModuleDirectory()}/Configs/";
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            configPath = $"{folderPath}{path}.json";
            Config = new T();

            Load();
        }

        internal bool Save()
        {
            string json = JsonSerializer.Serialize(Config, BaseConfig.JsonSerializerOptions);
            File.WriteAllText(configPath, json);

            return true;
        }

        internal bool Load()
        {
            if (File.Exists(configPath) == false)
            {
                Save();
                return false;
            }

            string json = File.ReadAllText(configPath);
            T? possibleConfig = JsonSerializer.Deserialize<T>(json, BaseConfig.JsonSerializerOptions);

            if (possibleConfig == null)
            {
                return false;
            }

            Config = possibleConfig;
            return true;
        }
    }
}
