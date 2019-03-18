using System;
using Domain.Model;

namespace Domain.Events
{
    public class OrderCreatedEvent
    {
        public OrderCreatedEvent(Guid id, CreditCard creditCard)
        {
            Id = id;
            CreditCard = creditCard;
        }

        public Guid Id { get; set; }
        public CreditCard CreditCard { get; set; }
    }
}
