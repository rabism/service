
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;
using System;
using stock.Models;
using MediatR;
using stock.Entity;
using stock.Commands;

namespace stock.Services
{
    public class CompanyDeleteConsumeService : BackgroundService
    {
        //  ConsumerConfig consumerConfig;
        // private readonly   IServiceScopeFactory _scopefactory;
        private readonly IServiceScopeFactory scopefactory;
        private readonly RabbitConfiguration configuration;
        readonly ILogger<CompanyDeleteConsumeService> logger;
        private IModel _channel;
        private IConnection _connection;

        public CompanyDeleteConsumeService(IServiceScopeFactory _scopefactory,RabbitConfiguration _configuration,ILogger<CompanyDeleteConsumeService> _logger)
        {
            scopefactory = _scopefactory;
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
                _channel.QueueDeclare(queue: configuration.DeleteCompanyQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
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
                    logger.LogInformation("delete content "+content);
                    var updatedDetail =System.Text.Json.JsonSerializer.Deserialize<Company>(content);
                    HandleMessage(updatedDetail);
                    _channel.BasicAck(ea.DeliveryTag, false);
                };
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                _channel.BasicConsume(configuration.DeleteCompanyQueueName, false, consumer);

            }
            catch (System.Exception ex)
            {
                 logger.LogError("Error occur on consuming message"+ex.Message);
            }
        }
        private void HandleMessage(Company updatedDetail)
        {
          try
          {
                using (var scope = scopefactory.CreateScope())
                {
                    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    logger.LogInformation("delete content "+System.Text.Json.JsonSerializer.Serialize(updatedDetail));
                    DeleteCompanyStockCommand command = new DeleteCompanyStockCommand
                    {
                        companyCode = updatedDetail.CompanyCode
                    };
                    mediator.Send(command);
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
