using Dapper;
using Npgsql;
using PurchaseLoaderApp.Models;
using PurchaseLoaderApp.Utilities.Logging;
using System;
using System.Configuration;
using System.Data;

namespace PurchaseLoaderApp
{
    /// <summary>
    /// Сервис для работы с базой данных.
    /// Содержит методы для сохранения данных о закупке и заказчиках.
    /// </summary>
    public class DatabaseService
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// Загружается из конфигурации приложения.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Конструктор класса DatabaseService.
        /// Проверяет наличие строки подключения в конфигурации.
        /// </summary>
        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PurchaseLoaderDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Строка подключения не найдена в конфигурации.");
            }
        }

        /// <summary>
        /// Сохраняет данные о закупке и связанных заказчиках в базу данных.
        /// Если закупка уже существует, данные обновляются.
        /// </summary>
        /// <param name="purchase">Объект закупки, который нужно сохранить.</param>
        public void SavePurchase(Purchase purchase)
        {
            try
            {
                Logger.Debug("Начало сохранения закупки в базу данных.");

                using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
                {
                    dbConnection.Open();
                    Logger.Debug("Соединение с базой данных установлено.");

                    CreateTablesIfNotExist(dbConnection);

                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        Logger.Debug("Транзакция начата.");

                        var existingPurchaseId = GetExistingPurchaseId(purchase.PurchaseNumber, dbConnection, transaction);

                        if (existingPurchaseId == null)
                        {
                            InsertPurchase(purchase, dbConnection, transaction);
                        }
                        else
                        {
                            UpdatePurchase(purchase, existingPurchaseId.Value, dbConnection, transaction);
                        }

                        InsertCustomers(purchase, dbConnection, transaction);

                        transaction.Commit();
                        Logger.Debug("Транзакция успешно завершена.");
                    }
                }

                Logger.Info("Закупка успешно сохранена в базу данных.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при сохранении закупки в базу данных.");
                throw;
            }
        }

        /// <summary>
        /// Проверяет наличие таблиц "purchases" и "customers" и создает их при отсутствии.
        /// </summary>
        /// <param name="dbConnection">Соединение с базой данных.</param>
        private void CreateTablesIfNotExist(IDbConnection dbConnection)
        {
            string createPurchasesTableQuery = @"
                CREATE TABLE IF NOT EXISTS purchases (
                    id SERIAL PRIMARY KEY,
                    purchase_number VARCHAR(50) NOT NULL,
                    purchase_name VARCHAR(200),
                    starting_price DECIMAL(10, 2),
                    publication_date DATE,
                    purchase_object_info TEXT
                )";
            dbConnection.Execute(createPurchasesTableQuery);
            Logger.Debug("Таблица 'purchases' проверена/создана.");

            string createCustomersTableQuery = @"
                CREATE TABLE IF NOT EXISTS customers (
                    id SERIAL PRIMARY KEY,
                    organization_name VARCHAR(200),
                    inn VARCHAR(50),
                    reg_num VARCHAR(50),
                    cons_registry_num VARCHAR(50),
                    purchase_id INT REFERENCES purchases(id) ON DELETE CASCADE
                )";
            dbConnection.Execute(createCustomersTableQuery);
            Logger.Debug("Таблица 'customers' проверена/создана.");
        }

        /// <summary>
        /// Проверяет, существует ли закупка с указанным номером.
        /// </summary>
        /// <param name="purchaseNumber">Номер закупки.</param>
        /// <param name="dbConnection">Соединение с базой данных.</param>
        /// <param name="transaction">Текущая транзакция.</param>
        /// <returns>ID существующей закупки или null, если она отсутствует.</returns>
        private int? GetExistingPurchaseId(string purchaseNumber, IDbConnection dbConnection, IDbTransaction transaction)
        {
            string selectQuery = "SELECT id FROM purchases WHERE purchase_number = @PurchaseNumber";
            return dbConnection.QueryFirstOrDefault<int?>(selectQuery, new { PurchaseNumber = purchaseNumber }, transaction);
        }

        /// <summary>
        /// Вставляет новую закупку в базу данных.
        /// </summary>
        /// <param name="purchase">Объект закупки для вставки.</param>
        /// <param name="dbConnection">Соединение с базой данных.</param>
        /// <param name="transaction">Текущая транзакция.</param>
        private void InsertPurchase(Purchase purchase, IDbConnection dbConnection, IDbTransaction transaction)
        {
            string insertQuery = @"
                INSERT INTO purchases (purchase_number, purchase_name, starting_price, publication_date, purchase_object_info)
                VALUES (@PurchaseNumber, @PurchaseName, @StartingPrice, @PublicationDate, @PurchaseObjectInfo)
                RETURNING id";
            purchase.Id = dbConnection.ExecuteScalar<int>(insertQuery, purchase, transaction);
            Logger.Debug($"Закупка с номером {purchase.PurchaseNumber} вставлена с ID: {purchase.Id}");
        }

        /// <summary>
        /// Обновляет данные существующей закупки в базе данных.
        /// </summary>
        /// <param name="purchase">Объект закупки для обновления.</param>
        /// <param name="purchaseId">ID существующей закупки.</param>
        /// <param name="dbConnection">Соединение с базой данных.</param>
        /// <param name="transaction">Текущая транзакция.</param>
        private void UpdatePurchase(Purchase purchase, int purchaseId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            purchase.Id = purchaseId;
            string updateQuery = @"
                UPDATE purchases
                SET purchase_name = @PurchaseName,
                    starting_price = @StartingPrice,
                    publication_date = @PublicationDate,
                    purchase_object_info = @PurchaseObjectInfo
                WHERE id = @Id";
            dbConnection.Execute(updateQuery, purchase, transaction);
            Logger.Debug($"Закупка с номером {purchase.PurchaseNumber} обновлена.");
        }

        /// <summary>
        /// Вставляет заказчиков, связанных с закупкой.
        /// </summary>
        /// <param name="purchase">Объект закупки с заказчиками.</param>
        /// <param name="dbConnection">Соединение с базой данных.</param>
        /// <param name="transaction">Текущая транзакция.</param>
        private void InsertCustomers(Purchase purchase, IDbConnection dbConnection, IDbTransaction transaction)
        {
            if (purchase.Customers == null || purchase.Customers.Count == 0)
            {
                Logger.Debug("Заказчики отсутствуют. Пропускаем вставку.");
                return;
            }

            string insertQuery = @"
                INSERT INTO customers (organization_name, inn, reg_num, cons_registry_num, purchase_id)
                VALUES (@OrganizationName, @INN, @RegNum, @ConsRegistryNum, @PurchaseId)";
            foreach (var customer in purchase.Customers)
            {
                customer.PurchaseId = purchase.Id;
                dbConnection.Execute(insertQuery, customer, transaction);
                Logger.Debug($"Заказчик {customer.OrganizationName} добавлен для закупки с ID: {purchase.Id}");
            }
        }

        /// <summary>
        /// Обрезает строку до указанной длины.
        /// </summary>
        /// <param name="value">Строка для обрезки.</param>
        /// <param name="maxLength">Максимальная длина строки.</param>
        /// <returns>Обрезанная строка.</returns>
        private string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }
    }
}
