using Domain.Events;
using Domain.Model;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _configuration = configuration;
            var mongoClient = new MongoClient(_configuration["MongoSettings:Connection"]);
            _orderCollection = mongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]).GetCollection<Order>("Orders");
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

            return Ok();
        }
    }
}
