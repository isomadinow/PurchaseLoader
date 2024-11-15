using PurchaseLoaderApp.Models;
using PurchaseLoaderApp.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace PurchaseLoaderApp.Loaders
{
    /// <summary>
    /// Загрузчик данных закупок из XML-файлов.
    /// </summary>
    public class XmlPurchaseLoader : IPurchaseLoader
    {
        /// <summary>
        /// Путь к XML-файлу, содержащему данные о закупке.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Инициализирует экземпляр <see cref="XmlPurchaseLoader"/> с указанием пути к XML-файлу.
        /// </summary>
        /// <param name="filePath">Путь к XML-файлу.</param>
        public XmlPurchaseLoader(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Загружает данные о закупке из XML-файла.
        /// </summary>
        /// <returns>Объект <see cref="Purchase"/>, содержащий данные о закупке.</returns>
        public Purchase LoadPurchase()
        {
            Logger.Debug("Начало загрузки закупки из XML-файла: " + _filePath);

            try
            {
                var xdoc = XDocument.Load(_filePath);
                Logger.Debug("XML-файл успешно загружен.");

                XNamespace ns2 = "http://zakupki.gov.ru/oos/export/1";
                XNamespace ns = "http://zakupki.gov.ru/oos/types/1";

                var notification = xdoc.Element(ns2 + "export")?.Element(ns2 + "fcsNotificationEF");

                if (notification == null)
                {
                    Logger.Error("Не удалось найти элемент fcsNotificationEF в XML.");
                    throw new Exception("Неверный формат XML.");
                }

                Logger.Debug("Элемент fcsNotificationEF найден, начинаем извлечение данных закупки.");

                var purchase = new Purchase
                {
                    PurchaseNumber = notification.Element(ns + "purchaseNumber")?.Value,
                    PurchaseName = notification.Element(ns + "purchaseObjectInfo")?.Value,
                    StartingPrice = decimal.Parse(
                        notification.Element(ns + "lot")?.Element(ns + "maxPrice")?.Value ?? "0",
                        CultureInfo.InvariantCulture
                    ),
                    PublicationDate = DateTime.Parse(
                        notification.Element(ns + "docPublishDate")?.Value ?? "2000-01-01",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal
                    ),
        
                    Customers = ExtractCustomers(notification, ns)
                };

                Logger.Info("Данные закупки успешно извлечены из XML.");
                return purchase;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при загрузке закупки из XML.");
                throw;
            }
        }

        /// <summary>
        /// Извлекает данные заказчиков из XML-узла закупки.
        /// </summary>
        /// <param name="notification">Элемент XML, содержащий данные о закупке.</param>
        /// <param name="ns">Пространство имен XML.</param>
        /// <returns>Список объектов <see cref="Customer"/>, представляющих заказчиков.</returns>
        private List<Customer> ExtractCustomers(XElement notification, XNamespace ns)
        {
            Logger.Debug("Начинаем извлечение данных заказчиков из XML.");

            var customers = new List<Customer>();

            var customerRequirements = notification.Element(ns + "lot")?
                .Element(ns + "customerRequirements")?
                .Elements(ns + "customerRequirement");

            if (customerRequirements != null)
            {
                Logger.Debug("Обнаружены требования заказчиков, начинаем извлечение данных.");

                foreach (var requirement in customerRequirements)
                {
                    var customerElement = requirement.Element(ns + "customer");

                    if (customerElement != null)
                    {
                        var customer = new Customer
                        {
                            OrganizationName = customerElement.Element(ns + "fullName")?.Value,
                            INN = customerElement.Element(ns + "INN")?.Value,
                        };

                        customers.Add(customer);
                        Logger.Debug("Данные заказчика добавлены: " + customer.OrganizationName);
                    }
                }
            }
            else
            {
                Logger.Debug("Не найдено элементов customerRequirements в XML.");
            }

            Logger.Debug("Извлечение данных заказчиков завершено.");
            return customers;
        }
    }
}
