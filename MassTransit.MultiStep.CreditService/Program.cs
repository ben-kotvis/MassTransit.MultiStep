using System;

namespace MassTransit.MultiStep.CreditService
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
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

            bus.Start();

            Console.WriteLine("Press any key to exit");
            Console.Read();

            bus.Stop();
        }
    }
}
