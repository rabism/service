using stock.Models;
using stock.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace stock.Services
{
    public class StockUpdateSenderService :IStockUpdateSenderService
    {
        readonly RabbitConfiguration configuration;
        readonly ILogger<StockUpdateSenderService> logger;
        private  Lazy<IConnection> _connection;
        public StockUpdateSenderService(RabbitConfiguration _configuration, ILogger<StockUpdateSenderService> _logger)
        {
            configuration = _configuration;
            logger = _logger;

            CreateConnection();
        }
      public  void SendAddStock(string companyCode,string exchangeName,decimal currentStockPrice)
        {
            try
            {
                if (IsConnectionExist())
                {
                    using (var channel = _connection.Value.CreateModel())
                    {
                       logger.LogInformation("Successfully create channel!!");
                        channel.QueueDeclare(queue: configuration.AddStockQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                        var sendData = new {
                            CompanyCode=companyCode,
                            StockPrice=currentStockPrice,
                            ExchangeName=exchangeName

                        };
                        var json = JsonSerializer.Serialize(sendData);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(exchange: "", routingKey: configuration.AddStockQueueName, basicProperties: null, body: body);
                        logger.LogInformation("Successfully publish message!!");
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError("Error occured during publish message "+ex.Message);
            }
           
        }
      

        private void CreateConnection()
        {
            try
            {
                 logger.LogInformation("config property "+JsonSerializer.Serialize(configuration));

                var factory = new ConnectionFactory
                {
                    HostName = configuration.Hostname,
                    UserName = configuration.UserName,
                    Password = configuration.Password,
                    VirtualHost="/",
                    Port=AmqpTcpEndpoint.UseDefaultPort
                    
                };
                _connection = new Lazy<IConnection>(factory.CreateConnection());
                logger.LogInformation("Successfully made the connection");
            }
            catch (Exception ex)
            {
                logger.LogError("Error occured on connection "+ex.Message);
            }
        }

        private bool IsConnectionExist()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}