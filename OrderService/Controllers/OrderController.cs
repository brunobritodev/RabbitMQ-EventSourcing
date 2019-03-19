using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Events;
using Domain.Model;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace OrderService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Order> _orderCollection;

        public OrderController(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGOCONNECTION") ?? _configuration.GetSection("MongoSettings").GetSection("Connection").Value);
            _orderCollection = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASENAME") ?? _configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value).GetCollection<Order>("Orders");
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {

            var orders = await _orderCollection.FindAsync(Builders<Order>.Filter.Empty);

            return Ok(orders.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Order order)
        {
            // Businness rules, validations... etc..
            order.CreateNew();
            await _orderCollection.InsertOneAsync(order);

            // If everything goes fine, then
            using (var bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITCONNECTION") ?? _configuration.GetSection("RabbitSettings").GetSection("Connection").Value))
            {
                bus.Publish(new OrderCreatedEvent(order.Id, order.CreditCard));
            }

            return Ok(true);
        }
    }
}
