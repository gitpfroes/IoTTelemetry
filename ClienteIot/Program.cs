using Actors.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClienteIot
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var thing = ActorProxy.Create<IThing>(new ActorId(1), new Uri("fabric:/IoTTelemetry/ThingActorService"));
                thing.ActivateMe("sudeste", 1).Wait();
                thing.SendTelemetryAsync(new Domain.ThingTelemetry() { DevelopedFault = true, DeviceId = "1", Region = "sudeste" }).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async Task Test(int i)
        {
            CancellationToken cancellationToken;
            var actor = ActorProxy.Create<IActors>(new ActorId(i), new Uri("fabric:/IoTTelemetry/ActorsActorService"));
            await actor.SetCountAsync(i, cancellationToken);
            var bla = await actor.GetCountAsync(cancellationToken);
            Console.WriteLine(bla);
        }
    }
}
