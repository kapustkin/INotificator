using System;

namespace INotificator.Context.Models
{
    public class Product
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Price { get; set; }
    }
}