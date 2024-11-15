using PurchaseLoaderApp.Models;

namespace PurchaseLoaderApp.Loaders
{
    /// <summary>
    /// Интерфейс загрузчика закупок.
    /// </summary>
    public interface IPurchaseLoader
    {
        Purchase LoadPurchase();
    }
}
