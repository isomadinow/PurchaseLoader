using System;

namespace PurchaseLoaderApp.Utilities.Logging
{
    /// <summary>
    /// Простой класс логирования, поддерживающий различные уровни сообщений (Debug, Info, Error).
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Указывает, включен ли режим отладки.
        /// Если <c>true</c>, сообщения уровня Debug будут выводиться в консоль.
        /// </summary>
        public static bool IsDebugMode { get; set; }

        /// <summary>
        /// Выводит сообщение уровня Debug в консоль, если включен режим отладки.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        public static void Debug(string message)
        {
            if (IsDebugMode)
            {
                Console.WriteLine($"DEBUG: {message}");
            }
        }

        /// <summary>
        /// Выводит информационное сообщение уровня Info в консоль.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        public static void Info(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }

        /// <summary>
        /// Выводит сообщение об ошибке уровня Error в консоль.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        public static void Error(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }

        /// <summary>
        /// Выводит сообщение об ошибке уровня Error с исключением в консоль.
        /// </summary>
        /// <param name="ex">Исключение, связанное с ошибкой.</param>
        /// <param name="message">Сообщение для логирования.</param>
        public static void Error(Exception ex, string message)
        {
            Console.WriteLine($"ERROR: {message} Исключение: {ex.Message}");
        }
    }
}
