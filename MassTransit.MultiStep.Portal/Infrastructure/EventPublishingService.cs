using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransit.MultiStep.Portal.Infrastructure
{

    public class EventPublishingService : IDisposable
    {
        private readonly IBusControl _busControl;

        public EventPublishingService()
        {
                _busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://abi-rabbit"), h =>
                    {
                    });
                });
            _busControl.Start();
        }

        public async Task Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            await _busControl.Publish<TMessage>(message);
        }
        /*
        public async Task Reschedule<TMessage>(TimeSpan delay, TMessage message) where TMessage : class
        {
            await _busControl.ScheduleSend<TMessage>(new Uri($"{_configuration["ServiceBus:Path"]}/quartz"), delay, message);
        }
        */
        public void Dispose()
        {
            if (_busControl != null)
            {
                _busControl.Stop(TimeSpan.FromSeconds(30));
            }
        }

    }
}
