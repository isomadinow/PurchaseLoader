using PurchaseLoaderApp.Loaders;
using System;

namespace PurchaseLoaderApp.Factory
{
    /// <summary>
    /// Фабрика для создания загрузчиков закупок.
    /// </summary>
    public static class PurchaseLoaderFactory
    {
        public static IPurchaseLoader CreateLoader(string sourceType, string source)
        {
            switch (sourceType.ToLower())
            {
                case "xml":
                    return new XmlPurchaseLoader(source);
                case "web":
                    return new WebPurchaseLoader(source);
                default:
                    throw new ArgumentException("Неизвестный тип источника: " + sourceType);
            }
        }
    }
}
