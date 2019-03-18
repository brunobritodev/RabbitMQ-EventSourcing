using Domain.Model;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendApp.Interfaces
{
    public interface IPaymentService
    {
        [Get("/payments")]
        Task<List<Payment>> GetPayments();
    }
}