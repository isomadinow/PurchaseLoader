using CommandLine;
using PurchaseLoaderApp.Factory;
using PurchaseLoaderApp.Utilities;
using PurchaseLoaderApp.Utilities.Config;
using PurchaseLoaderApp.Utilities.Logging;
using System;

namespace PurchaseLoaderApp
{
    /// <summary>
    /// Главный класс приложения, который управляет процессом загрузки закупок и обработки данных.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Глобальная конфигурация приложения, загружаемая из JSON.
        /// </summary>
        private static AppConfig _applicationConfig;

        /// <summary>
        /// Точка входа в приложение.
        /// </summary>
        /// <param name="commandLineArguments">Аргументы командной строки.</param>
        /// <returns>Код завершения приложения: 0 - успешное выполнение, 1 - ошибка.</returns>
        static int Main(string[] commandLineArguments)
        {
            _applicationConfig = AppConfigLoader.LoadConfiguration();

            return Parser.Default.ParseArguments<Options>(commandLineArguments)
                .MapResult(
                    (Options parsedOptions) => ExecuteApplicationLogic(OptionsHandler.HandleMissingOptions(parsedOptions, _applicationConfig)),
                    parseErrors => 1);
        }

        /// <summary>
        /// Запускает основную логику приложения: загрузку данных закупки, их отображение и сохранение.
        /// </summary>
        /// <param name="processedOptions">Опции, указанные пользователем через командную строку или интерактивно.</param>
        /// <returns>Код завершения приложения: 0 - успешное выполнение, 1 - ошибка.</returns>
        static int ExecuteApplicationLogic(Options processedOptions)
        {
            Logger.IsDebugMode = processedOptions.Debug;
            Logger.Info("Приложение запущено.");

            try
            {
                var purchaseLoader = PurchaseLoaderFactory.CreateLoader(processedOptions.SourceType, processedOptions.Source);
                var purchaseData = purchaseLoader.LoadPurchase();

                Logger.Debug("Загруженные данные закупки:");
                Logger.Debug($"Номер закупки: {purchaseData.PurchaseNumber}");
                Logger.Debug($"Наименование закупки: {purchaseData.PurchaseName}");
                Logger.Debug($"Начальная цена: {purchaseData.StartingPrice}");
                Logger.Debug($"Дата публикации: {purchaseData.PublicationDate}");
                Logger.Debug("Заказчики:");

                if (purchaseData.Customers != null && purchaseData.Customers.Count > 0)
                {
                    foreach (var customer in purchaseData.Customers)
                    {
                        Logger.Debug($" - Название организации: {customer.OrganizationName}");
                        Logger.Debug($"   ИНН: {customer.INN}");
                    }
                }
                else
                {
                    Logger.Debug("Заказчики не найдены.");
                }

               var databaseService = new DatabaseService();
               databaseService.SavePurchase(purchaseData);

                Logger.Info("Приложение успешно завершило работу.");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Произошла ошибка при выполнении приложения.");
                return 1;
            }
            finally
            {
                Logger.Info("Нажмите Enter для завершения работы программы...");
                Console.ReadLine();
            }

            return 0;
        }
    }
}
