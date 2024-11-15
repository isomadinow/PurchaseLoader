using PurchaseLoaderApp.Utilities.Config;
using PurchaseLoaderApp.Utilities.Logging;
using System;

namespace PurchaseLoaderApp.Utilities
{
    /// <summary>
    /// Класс для обработки опций командной строки и взаимодействия с пользователем.
    /// </summary>
    public static class OptionsHandler
    {
        public static Options HandleMissingOptions(Options opts, AppConfig config)
        {
            if (string.IsNullOrEmpty(opts.SourceType))
            {
                Logger.Info("Выберите тип источника данных:");
                Logger.Info("1. Xml");
                Logger.Info("2. Web");

                string choiceLoader = Console.ReadLine();
                switch (choiceLoader)
                {
                    case "1":
                    case "":
                        opts.SourceType = "Xml";
                        Logger.Info("Тип источника выбран: Xml.");
                        break;
                    case "2":
                        opts.SourceType = "Web";
                        Logger.Info("Тип источника выбран: Web.");
                        break;
                    default:
                        Logger.Info("Неверный выбор. Пожалуйста, выберите 1 или 2.");
                        return HandleMissingOptions(opts, config);
                }
            }

            if (string.IsNullOrEmpty(opts.Source))
            {
                if (opts.SourceType == "Xml")
                {
                    Logger.Info($"Использовать путь по умолчанию для XML (Files/.xml)? (Нажмите Enter для использования по умолчанию или введите свой путь)");
                    string inputPath = Console.ReadLine();
                    opts.Source = string.IsNullOrEmpty(inputPath) ? config.DefaultPaths.XmlFilePath : inputPath;
                    Logger.Info($"Путь для XML установлен: {opts.Source}");
                }
                else if (opts.SourceType == "Web")
                {
                    Logger.Info($"Использовать URL по умолчанию для Web ({config.DefaultPaths.WebUrl})? (Нажмите Enter для использования по умолчанию или введите свой URL)");
                    string inputUrl = Console.ReadLine();
                    opts.Source = string.IsNullOrEmpty(inputUrl) ? config.DefaultPaths.WebUrl : inputUrl;
                    Logger.Info($"URL для Web установлен: {opts.Source}");
                }
            }

            return opts;
        }
    }
}
