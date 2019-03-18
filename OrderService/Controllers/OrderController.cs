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

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGOCONNECTION") ?? _configuration.GetSection("MongoSettings").GetSection("Connection").Value);
            var orderCollection = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASENAME") ?? _configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value).GetCollection<Order>("Orders");

            var orders = await orderCollection.FindAsync(Builders<Order>.Filter.Empty);

            return Ok(orders.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Order order)
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGOCONNECTION") ?? _configuration.GetSection("MongoSettings").GetSection("Connection").Value);
            var orderCollection = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASENAME") ?? _configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value).GetCollection<Order>("Orders");


            // Businness rules, validations... etc..
            order.CreateNew();
            await orderCollection.InsertOneAsync(order);

            // If everything goes fine, then
            using (var bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITCONNECTION") ?? _configuration.GetSection("RabbitSettings").GetSection("Connection").Value))
            {
                bus.Publish(new OrderCreatedEvent(order.Id, order.CreditCard));
            }

            return Ok(true);
        }
    }
}
