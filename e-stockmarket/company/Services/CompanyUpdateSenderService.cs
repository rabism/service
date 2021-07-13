using company.Models;
using company.Entity;
using company.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace company.Services
{
    public class CompanyUpdateSenderService :ICompanyUpdateSenderService
    {
        readonly RabbitConfiguration configuration;
        readonly ILogger<CompanyUpdateSenderService> logger;
        private  Lazy<IConnection> _connection;
        public CompanyUpdateSenderService(RabbitConfiguration _configuration, ILogger<CompanyUpdateSenderService> _logger)
        {
            configuration = _configuration;
            logger = _logger;

            CreateConnection();
        }
      public  void SendAddCompany(Company company)
        {
            try
            {
                if (IsConnectionExist())
                {
                    using (var channel = _connection.Value.CreateModel())
                    {
                       logger.LogInformation("Successfully create channel!!");
                        channel.QueueDeclare(queue: configuration.AddCompanyQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                        var sendData = new 
                        {
                            CompanyCode = company.CompanyCode,
                            Exchanges = company.CompanyExchange.Select(x => new
                            {
                                ExchangeName = x.ExchangeName
                            }).ToList()
                        };
                        var json = JsonSerializer.Serialize(sendData);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(exchange: "", routingKey: configuration.AddCompanyQueueName, basicProperties: null, body: body);
                        logger.LogInformation("Successfully publish message!!");
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError("Error occured during publish message "+ex.Message);
            }
           
        }
      public  void SendDeleteCompany(string companyCode)
        {
            try
            {
                if (IsConnectionExist())
                {
                    using (var channel = _connection.Value.CreateModel())
                    {
                        
                        channel.QueueDeclare(queue: configuration.DeleteCompanyQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                        var sendData = new  { CompanyCode = companyCode };
                        var json = JsonSerializer.Serialize(sendData);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(exchange: "", routingKey: configuration.DeleteCompanyQueueName, basicProperties: null, body: body);
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