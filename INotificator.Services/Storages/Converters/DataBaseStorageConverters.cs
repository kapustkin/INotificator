using System;
using INotificator.Common.Models;
using INotificator.Common.Models.Services.BaseProductService;

namespace INotificator.Services.Storages.Converters
{
    /// <summary>
    /// Конвертеры для моделей DB -> Common
    /// </summary>
    public static class DataBaseStorageConverters
    {
        public static Product MapToProduct(this Context.Models.Product product)
        {
            if (product == null) return null;
            return new Product()
            {
                Date = product.Date,
                Name = product.Name,
                Price = product.Price,
                Source = product.Source,
                Url = product.Url
            };
        }
        
        public static Context.Models.Product MapToContextProduct(this Product product)
        {
            if (product == null) return null;
            return new Context.Models.Product()
            {
                Id = Guid.NewGuid().ToString(),
                Date = product.Date,
                Name = product.Name,
                Price = product.Price,
                Source = product.Source,
                Url = product.Url
            };
        }
    }
}