using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Models;
using INotificator.Context;
using INotificator.Services.Storages.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace INotificator.Services.Storages
{
    public class DataBaseStorage: IStorage<Product>
    {
        private ApplicationContext _context;
        
        public DataBaseStorage(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Product> GetItem(string name)
        {
            var product = await _context.Products.OrderByDescending(s=>s.Date).FirstOrDefaultAsync(s => s.Name == name);
            return product?.MapToProduct();
        }

        public async Task<bool> AddItem(Product item)
        {
            var entity = await _context.Products.AddAsync(item.MapToContextProduct()).AsTask();
            await _context.SaveChangesAsync();
            return entity?.Entity.Id != null;
        }

        public Task<bool> SaveItems(ICollection<Product> products)
        {
            throw new System.NotImplementedException();
        }
    }
}