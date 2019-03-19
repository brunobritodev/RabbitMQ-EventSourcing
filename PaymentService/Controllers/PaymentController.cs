using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Payment> _payCollection;

        public PaymentsController(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGOCONNECTION") ?? _configuration.GetSection("MongoSettings").GetSection("Connection").Value);
            _payCollection = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASENAME") ?? _configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value).GetCollection<Payment>("Payments");

            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
        {
            var payments = await _payCollection.FindAsync(Builders<Payment>.Filter.Empty);

            return Ok(payments.ToList());
        }
    }
}
