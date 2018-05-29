using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;

namespace MassTransit.MultiStep.CommonBusSetup
{
    public class BusManager : IDisposable
    {
        private IBusControl _bus;

        public IBus BusAccess { get { return _bus; } }

        public BusManager()
        {

        }


        public void Start(Action<IReceiveEndpointConfigurator> configurator, string queueName)
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://abi-rabbit"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.UseInMemoryScheduler();

                if (!string.IsNullOrEmpty(queueName))
                {
                    sbc.ReceiveEndpoint(host, queueName, ep =>
                    {
                    //ep.UseRetry(i => i.Exponential(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)));
                    ep.PrefetchCount = 8;
                        configurator(ep);
                    });
                }

            });

            _bus.Start();
        }

        public void Dispose()
        {
            if(_bus != null)
            {
                _bus.Stop();
            }
        }
    }
}
