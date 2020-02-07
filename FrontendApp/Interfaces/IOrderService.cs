using Domain.Model;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendApp.Interfaces
{
    public interface IOrderService
    {
        [Get("/order")]
        Task<List<Order>> GetOrders();

        [Post("/order")]
        Task SaveOrders([Body] Order order);
    }
}
