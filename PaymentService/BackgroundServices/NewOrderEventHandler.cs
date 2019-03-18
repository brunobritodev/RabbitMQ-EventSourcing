using System;
using System.Threading;
using System.Threading.Tasks;
using CreditCardValidator;
using Domain.Events;
using Domain.Model;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace PaymentService.BackgroundServices
{
    public class NewOrderEventHandler : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IBus _bus;

        public NewOrderEventHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITCONNECTION") ?? _configuration.GetSection("RabbitSettings").GetSection("Connection").Value);
            _bus.Subscribe<OrderCreatedEvent>("PaymentGateway", ProccessPayment);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
            _bus.Dispose();
        }

        private void ProccessPayment(OrderCreatedEvent order)
        {
            var payment = new Payment(order.Id, order.CreditCard.CardNumber, order.CreditCard.Expiration, order.CreditCard.Name);
            var detector = new CreditCardDetector(order.CreditCard.CardNumber);
            if (detector.IsValid())
                payment.Apply(detector.BrandName);
            else
                payment.Deny();

            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGOCONNECTION") ?? _configuration.GetSection("MongoSettings").GetSection("Connection").Value);
            var payCollection = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASENAME") ?? _configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value).GetCollection<Payment>("Payments");


            // Businness rules, validations... etc..
            payCollection.InsertOne(payment);


        }
    }


}
