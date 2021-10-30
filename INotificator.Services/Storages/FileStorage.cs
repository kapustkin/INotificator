using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Models;
using INotificator.Common.Models.Services.BaseProductService;
using Microsoft.Extensions.Logging;

namespace INotificator.Services.Storages
{
    public class FileStorage : IStorage<Product>
    {
        private readonly string _fileName = $"{AppDomain.CurrentDomain.BaseDirectory}storage.json";
        private ICollection<Product> _storage;

        private static readonly object LockObject = new object();

        public FileStorage(ILogger<FileStorage> logger)
        {
            logger.LogDebug($"Storage: {_fileName}");

            if (File.Exists(_fileName))
            {
                using FileStream openStream = File.OpenRead(_fileName);
                _storage = JsonSerializer.DeserializeAsync<IList<Product>>(openStream).GetAwaiter().GetResult();
                openStream.Close();
            }
            else
            {
                _storage = new List<Product>();
            }
        }

        /// </inheritdoc>
        public Task<Product> GetItem(string name)
        {
            return Task.FromResult(_storage?.FirstOrDefault(s => s.Name == name));
        }

        public Task<bool> AddItem(Product item)
        {
            if (GetItem(item.Name) == null)
            {
                _storage.Add(item);
                lock (LockObject)
                {
                    using var createStream = File.Create(_fileName);
                    JsonSerializer.SerializeAsync(createStream, _storage).GetAwaiter().GetResult();
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);;
        }

        /// </inheritdoc>
        public async Task<bool> SaveItems(ICollection<Product> products)
        {
            try
            {
                _storage = products;
                await using var createStream = File.Create(_fileName);
                await JsonSerializer.SerializeAsync(createStream, _storage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}