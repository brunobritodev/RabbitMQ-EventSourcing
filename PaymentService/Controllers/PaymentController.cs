using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<Payment> _payCollection;

        public PaymentsController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
        {
            StartMongoConnection();
            var payments = await _payCollection.FindAsync(Builders<Payment>.Filter.Empty);

            return Ok(payments.ToList());
        }

        private void StartMongoConnection()
        {
            var mongoClient = new MongoClient(_configuration["MongoSettings:Connection"]);
            _payCollection = mongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]).GetCollection<Payment>("Payments");
        }
    }
}
