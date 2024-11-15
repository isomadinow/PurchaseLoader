using System;
using System.Collections.Generic;

namespace PurchaseLoaderApp.Models
{
    /// <summary>
    /// Класс, представляющий сущность закупки.
    /// </summary>
    public class Purchase
    {
        /// <summary>
        /// Уникальный идентификатор закупки в базе данных.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер закупки, уникальный идентификатор на платформе.
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// Наименование закупки.
        /// </summary>
        public string PurchaseName { get; set; }

        /// <summary>
        /// Начальная цена закупки.
        /// </summary>
        public decimal StartingPrice { get; set; }

        /// <summary>
        /// Дата публикации информации о закупке.
        /// </summary>
        public DateTime PublicationDate { get; set; }

        /// <summary>
        /// Список заказчиков, связанных с закупкой.
        /// </summary>
        public ICollection<Customer> Customers { get; set; }
    }
}
