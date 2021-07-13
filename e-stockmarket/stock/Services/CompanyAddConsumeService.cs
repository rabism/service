
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
    public class CompanyAddConsumeService : BackgroundService
    {
        //  ConsumerConfig consumerConfig;
        // private readonly   IServiceScopeFactory _scopefactory;
       
        private readonly RabbitConfiguration configuration;
        readonly ILogger<CompanyAddConsumeService> logger;
        private readonly IServiceScopeFactory scopefactory;
        private IModel _channel;
        private IConnection _connection;

        public CompanyAddConsumeService(IServiceScopeFactory _scopefactory,RabbitConfiguration _configuration,ILogger<CompanyAddConsumeService> _logger)
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
                _channel.QueueDeclare(queue: configuration.AddCompanyQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
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
                logger.LogInformation("setconsumer called..!!");
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                    logger.LogInformation("content of message"+content);
                    var updatedDetail =System.Text.Json.JsonSerializer.Deserialize<Company>(content);
                    logger.LogInformation("deserilize data"+ System.Text.Json.JsonSerializer.Serialize(updatedDetail));
                    HandleMessage(updatedDetail);
                    _channel.BasicAck(ea.DeliveryTag, false);
                };
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                _channel.BasicConsume(configuration.AddCompanyQueueName, false, consumer);

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
               logger.LogInformation("Handle Method called!!");
               var st = System.Text.Json.JsonSerializer.Serialize(updatedDetail);
               logger.LogInformation("data>>>>>>"+st);
                using (var scope = scopefactory.CreateScope())
                {
                    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    UpdateCompanyCommand command = new UpdateCompanyCommand
                    {
                        Company = updatedDetail
                    };
                    mediator.Send(command);
                }
               
          }
          catch(Exception ex)
          {
              logger.LogError("Error occur on adding company!! "+ex.Message);
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
