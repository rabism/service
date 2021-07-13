using company.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;
using company.Entity;
using System;

namespace company.Services
{
    public class StockUpdateConsumeService : BackgroundService
    {
        //  ConsumerConfig consumerConfig;
        // private readonly   IServiceScopeFactory _scopefactory;
       // private readonly ICompanyService companyService;
        private readonly RabbitConfiguration configuration;
        readonly ILogger<StockUpdateConsumeService> logger;
        private IModel _channel;
        private IConnection _connection;
        private readonly IServiceScopeFactory scopefactory;

        public StockUpdateConsumeService(IServiceScopeFactory _scopefactory,RabbitConfiguration _configuration,ILogger<StockUpdateConsumeService> _logger)
        {
            //_scopefactory = scopefactory;
            //companyService = _companyService;
            scopefactory=_scopefactory;
            configuration = _configuration;
            logger=_logger;
            InitializeListner();
        }


         private void InitializeListner()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration.Hostname,
                    UserName = configuration.UserName,
                    Password = configuration.Password
                };

                _connection = factory.CreateConnection();
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: configuration.AddStockQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                logger.LogInformation("setup listner cnnection successfully");
            }
            catch(Exception ex)
            {
                logger.LogError("Error occur on setup listner connection"+ex.Message);
            }
           
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetConsumer(stoppingToken);
            return Task.CompletedTask;
        }

        private void SetConsumer(CancellationToken stoppingToken)
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var updatedDetail =System.Text.Json.JsonSerializer.Deserialize<CompanyExchange>(content);
                    HandleMessage(updatedDetail);
                    _channel.BasicAck(ea.DeliveryTag, false);
                };
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                _channel.BasicConsume(configuration.AddStockQueueName, false, consumer);

            }
            catch (System.Exception ex)
            {
                 logger.LogError("Error occur on consuming message"+ex.Message);
            }
        }
        private void HandleMessage(CompanyExchange updatedDetail)
        {
          try
          {
                using (var scope = scopefactory.CreateScope())
                {
                    ICompanyService companyService = scope.ServiceProvider.GetRequiredService<ICompanyService>();
                    companyService.UpdateCompanyExchangeCurrentPrice(updatedDetail.CompanyCode, updatedDetail.ExchangeName, updatedDetail.StockPrice);
                }
          }
          catch(Exception ex)
          {
              logger.LogError("Error occur on update company current price "+ex.Message);
          }
           
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            logger.LogInformation("consumer cancel method invoked!");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
             logger.LogInformation("consumer unregister method invoked!");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
             logger.LogInformation("consumer register  method invoked!");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
             logger.LogInformation("consumer shut down method invoked!");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            logger.LogInformation("connection shut down method invoked!");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
