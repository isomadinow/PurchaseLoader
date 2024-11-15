namespace PurchaseLoaderApp.Models
{
    /// <summary>
    /// Класс, представляющий сущность заказчика.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Уникальный идентификатор заказчика в базе данных.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название организации заказчика.
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Идентификационный номер налогоплательщика (ИНН) заказчика.
        /// </summary>
        public string INN { get; set; }

        /// <summary>
        /// Идентификатор связанной закупки в базе данных.
        /// </summary>
        public int PurchaseId { get; set; }

        /// <summary>
        /// Связанная закупка, к которой относится заказчик.
        /// </summary>
        public Purchase Purchase { get; set; }
    }
}
