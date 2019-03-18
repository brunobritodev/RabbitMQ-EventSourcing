using System;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class Order
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public CreditCard CreditCard { get; set; }

        public void CreateNew()
        {
            Id = Guid.NewGuid();
        }
    }
}
