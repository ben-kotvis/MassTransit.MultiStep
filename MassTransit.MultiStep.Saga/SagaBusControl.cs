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

namespace MassTransit.MultiStep.Saga
{
    public class SagaBusControl : IDisposable
    {
        private IBusControl _bus;

        public SagaBusControl()
        {

        }

        public void Start()
        {
            var machine = new UnderwritingStateMachine();
            var repository = new InMemorySagaRepository<UnderwritingState>();
            _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://abi-rabbit"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.UseInMemoryScheduler();

                sbc.ReceiveEndpoint(host, "underwriting-state-sag", ep =>
                {
                    ep.PrefetchCount = 8;
                    ep.StateMachineSaga<UnderwritingState>(machine, repository);
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
