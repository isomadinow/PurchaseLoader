using System;
using System.IO;
using System.Text.Json;

namespace PurchaseLoaderApp.Utilities.Config
{
    /// <summary>
    /// Класс для загрузки конфигурации приложения из файла JSON.
    /// </summary>
    public static class AppConfigLoader
    {
        public static AppConfig LoadConfiguration()
        {
          
            string configPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "pathсonfig.json");
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Файл конфигурации не найден: {configPath}");
            }

            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfig>(json);
            if (!Path.IsPathRooted(config.DefaultPaths.XmlFilePath))
            {
                config.DefaultPaths.XmlFilePath = Path.Combine(AppContext.BaseDirectory ,"..", "..", "..", config.DefaultPaths.XmlFilePath);
            }
            return config;
        }
    }


   
}
