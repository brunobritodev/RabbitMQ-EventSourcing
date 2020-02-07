using Bogus;
using Domain.Model;
using FrontendApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Refit;
using System.Threading.Tasks;

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
            var faker = new Faker();
           
            var creditCard = new CreditCard()
            {
                CVV = faker.Finance.CreditCardCvv(),
                CardNumber = faker.Finance.CreditCardNumber(),
                Expiration = faker.Date.Future().ToString("MM/yyyy"),
                Name = faker.Person.FullName
            };
            var order = new Order()
            {
                Amount = faker.Random.Number(500),
                Quantity = faker.Random.Number(5),
                Product = faker.Commerce.Product(),
                CreditCard = creditCard

            };
            return View("Index", order);
        }

        public IActionResult Payments()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            var service = RestService.For<IOrderService>(_configuration["OrderURI"]);
            return PartialView("_OrdersPartial", await service.GetOrders());
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder([FromForm] Order order)
        {
            var service = RestService.For<IOrderService>(_configuration["OrderURI"]);
            await service.SaveOrders(order);

            return Index();
        }


        [HttpGet]
        public async Task<ActionResult> GetPayments()
        {
            var paymentService = RestService.For<IPaymentService>(_configuration["PaymentURI"]);
            return PartialView("_PaymentsPartial", await paymentService.GetPayments());
        }
    }

}
