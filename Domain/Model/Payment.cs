using System;

namespace Domain.Model
{
    public class Payment
    {
        public Payment() { }
        public Payment(Guid orderId, string creditCardCardNumber, string creditCardExpiration, string creditCardName)
        {
            Id = Guid.NewGuid();
            Number = creditCardCardNumber.Length == 12 ? $"{creditCardCardNumber.Substring(0, 4)} {creditCardCardNumber.Substring(4, 2)}** **** {creditCardCardNumber.Substring(12)}" : creditCardCardNumber;
            OrderId = orderId;
            Expiration = creditCardExpiration;
            Name = creditCardName;
        }

        public Guid Id { get;  set; }
        public Guid OrderId { get;  set; }
        public string Name { get;  set; }
        public string IssueNetwork { get;  set; }
        public string Expiration { get;  set; }
        public string Number { get;  set; }
        public bool Success { get;  set; }
        public void Apply(string brand)
        {
            Success = true;
            IssueNetwork = brand;
        }

        public void Deny()
        {
            Success = false;
        }

    }
}
