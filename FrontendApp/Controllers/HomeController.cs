using Domain.Model;
using FrontendApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FrontendApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Payments()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            var service = RestService.For<IOrderService>(Environment.GetEnvironmentVariable("ORDERURI") ?? _configuration.GetSection("OrderURI").Value);
            return PartialView("_OrdersPartial", await service.GetOrders());
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder([FromForm] Order order)
        {
            var service = RestService.For<IOrderService>(Environment.GetEnvironmentVariable("ORDERURI") ?? _configuration.GetSection("OrderURI").Value);
            await service.SaveOrders(order);

            return Index();
        }


        [HttpGet]
        public async Task<ActionResult> GetPayments()
        {
            var paymentService = RestService.For<IPaymentService>(Environment.GetEnvironmentVariable("PAYMENTURI") ?? _configuration.GetSection("PaymentURI").Value);
            return PartialView("_PaymentsPartial", await paymentService.GetPayments());
        }
    }

}
