using MassTransit.Saga;
using System;
using MassTransit.RabbitMqTransport;

namespace MassTransit.MultiStep.Saga
{
    class Program
    {
        static void Main(string[] args)
        {
            var machine = new UnderwritingStateMachine();
            var repository = new InMemorySagaRepository<UnderwritingState>();
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
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

            bus.Start();

            Console.WriteLine("Press any key to exit");
            Console.Read();

            bus.Stop();
        }
    }
}
