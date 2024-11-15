using CommandLine;

namespace PurchaseLoaderApp
{
    /// <summary>
    /// Класс для хранения параметров командной строки.
    /// Используется библиотека CommandLine для парсинга аргументов.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Тип источника данных.
        /// Возможные значения: web, xml.
        /// </summary>
        [Option('t', "type", Required = false, HelpText = "Тип источника: web или xml.")]
        public string SourceType { get; set; }

        /// <summary>
        /// URL или путь к файлу, который необходимо обработать.
        /// Например, URL веб-страницы или путь к XML-файлу.
        /// </summary>
        [Option('s', "source", Required = false, HelpText = "URL или путь к файлу.")]
        public string Source { get; set; }

        /// <summary>
        /// Включение режима отладки.
        /// По умолчанию отладка выключена.
        /// </summary>
        [Option('d', "debug", Default = false, HelpText = "Включить режим отладки.")]
        public bool Debug { get; set; }
    }
}
