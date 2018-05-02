using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;

namespace MassTransit.MultiStep.CreditService
{
    public class CreditServiceBusControl : IDisposable
    {
        private IBusControl _bus;

        public CreditServiceBusControl()
        {

        }

        public void Start()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://abi-rabbit"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "credit-check", e =>
                {
                    e.PrefetchCount = 8;
                    e.Consumer<CreditCheckConsumer>();
                });

                sbc.ReceiveEndpoint(host, "request-assesment", e =>
                {
                    e.PrefetchCount = 8;
                    e.Consumer<RequestAssessmentConsumer>();
                });
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
