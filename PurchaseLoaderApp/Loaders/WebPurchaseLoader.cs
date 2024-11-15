using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PurchaseLoaderApp.Loaders;
using PurchaseLoaderApp.Models;
using PurchaseLoaderApp.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

/// <summary>
/// Класс для загрузки данных о закупке с веб-страницы с использованием Selenium.
/// </summary>
public class WebPurchaseLoader : IPurchaseLoader
{
    /// <summary>
    /// URL-адрес источника данных.
    /// </summary>
    private readonly string _url;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="WebPurchaseLoader"/>.
    /// </summary>
    /// <param name="url">URL-адрес источника данных.</param>
    public WebPurchaseLoader(string url)
    {
        _url = url;
    }

    /// <summary>
    /// Загружает данные о закупке с веб-страницы.
    /// </summary>
    /// <returns>Объект <see cref="Purchase"/>, содержащий данные о закупке.</returns>
    public Purchase LoadPurchase()
    {
        // Настройка ChromeOptions для headless-режима
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

        using (var driver = new ChromeDriver(options))
        {
            driver.Navigate().GoToUrl(_url);

            // TODO: Заменить на WebDriverWait для реального проекта, пока по моему мнению особо не требуется замена. 
            Thread.Sleep(5000);

            var purchase = new Purchase
            {
                PurchaseNumber = ParsePurchaseNumber(driver.FindElement(By.XPath("//*[@id='lot_detail']/div[2]/div[1]/div/div[1]/div[1]/div/a")).Text.Trim()),
                PurchaseName = driver.FindElements(By.XPath("//*[@id='lot-name-143303326']/div/span"))
                                    .FirstOrDefault()?.Text.Trim(),
                StartingPrice = ParsePrice(driver.FindElement(By.XPath("//*[@id='lot_detail']/div[2]/div[1]/div/div[2]/div/div[1]/span")).Text.Trim()),
                PublicationDate = ParsePublicationDate(
                    driver.FindElement(By.XPath("//*[@id='lot_detail']/div[2]/div[2]/div[1]/div[1]/div[2]/div/div[1]"))?.Text.Trim(),
                    driver.FindElement(By.XPath("//*[@id='lot_detail']/div[2]/div[2]/div[1]/div[1]/div[2]/div/div[2]"))?.Text.Trim()
                ),
                Customers = ParseCustomers(driver)
            };

            Logger.Debug($"Найден номер закупки: {purchase.PurchaseNumber}");
            Logger.Debug($"Наименование закупки: {purchase.PurchaseName}");
            Logger.Debug($"Найденная начальная цена: {purchase.StartingPrice}");
            Logger.Debug($"Дата публикации: {purchase.PublicationDate}");
            Logger.Debug($"Найдено заказчиков: {purchase.Customers.Count}");

            return purchase;
        }
    }

    /// <summary>
    /// Парсит номер закупки из текста.
    /// </summary>
    /// <param name="purchaseNumberText">Текст, содержащий номер закупки.</param>
    /// <returns>Только цифры из номера закупки.</returns>
    private string ParsePurchaseNumber(string purchaseNumberText)
    {
        var sanitizedNumber = new string(purchaseNumberText.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(sanitizedNumber) ? "UNKNOWN" : sanitizedNumber;
    }

    /// <summary>
    /// Парсит начальную цену закупки из текста.
    /// </summary>
    /// <param name="priceText">Текст, содержащий цену.</param>
    /// <returns>Числовое значение цены.</returns>
    private decimal ParsePrice(string priceText)
    {
        var sanitizedPrice = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray())
            .Replace(',', '.');

        return decimal.TryParse(sanitizedPrice, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var price) ? price : 0;
    }

    /// <summary>
    /// Парсит дату публикации закупки из двух частей текста.
    /// </summary>
    /// <param name="datePart1">Первая часть даты (например, "25.09.2019").</param>
    /// <param name="datePart2">Вторая часть времени (например, "08:55 МСК").</param>
    /// <returns>Объект <see cref="DateTime"/>, представляющий дату публикации.</returns>
    private DateTime ParsePublicationDate(string datePart1, string datePart2)
    {
        try
        {
            var fullDateText = $"{datePart1} {datePart2}".Trim();
            fullDateText = fullDateText.Replace("МСК", "").Trim();

            if (DateTime.TryParseExact(fullDateText, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var publicationDate))
            {
                return publicationDate;
            }
            else
            {
                Logger.Debug($"Не удалось преобразовать дату публикации: {fullDateText}");
                return DateTime.MinValue;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Ошибка при обработке даты публикации: {ex.Message}");
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Извлекает список заказчиков с веб-страницы.
    /// </summary>
    /// <param name="driver">Объект <see cref="IWebDriver"/> для взаимодействия с веб-страницей.</param>
    /// <returns>Список заказчиков, связанных с закупкой.</returns>
    private List<Customer> ParseCustomers(IWebDriver driver)
    {
        var customers = new List<Customer>();

        try
        {
            // Находим элементы, представляющие заказчиков
            var customerElements = driver.FindElements(By.XPath("//*[@id='customers-list']/div[2]/div[2]/div"));

            foreach (var element in customerElements)
            {
                try
                {
                    // Извлекаем название организации
                    var organizationName = element.FindElement(By.XPath("./div/a")).Text.Trim();

                    // Извлекаем ИНН
                    var innText = element.FindElement(By.XPath("./div/div")).Text
                        .Split(',')
                        .FirstOrDefault(part => part.Trim().StartsWith("ИНН:"))
                        ?.Replace("ИНН:", "").Trim();

                    // Добавляем заказчика в список
                    customers.Add(new Customer
                    {
                        OrganizationName = organizationName,
                        INN = innText
                    });

                }
                catch (NoSuchElementException ex)
                {
                    Logger.Error($"Ошибка при обработке заказчика: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Общая ошибка при извлечении заказчиков: {ex.Message}");
        }

        return customers;
    }
}
