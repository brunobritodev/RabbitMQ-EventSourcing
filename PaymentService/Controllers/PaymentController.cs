using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace PaymentService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGOCONNECTION") ?? _configuration.GetSection("MongoSettings").GetSection("Connection").Value);
            var payCollection = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASENAME") ?? _configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value).GetCollection<Payment>("Payments");

            var payments = await payCollection.FindAsync(Builders<Payment>.Filter.Empty);

            return Ok(payments.ToList());
        }
    }
}
