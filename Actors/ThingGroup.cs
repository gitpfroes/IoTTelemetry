using Actors.Interfaces;
using Domain;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    public class ThingGroup : Actor, IThingGroup
    {
        private ThingGroupState State = new ThingGroupState();
        public ThingGroup(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            State.devices = new List<ThingInfo>();
            State.faultsPerRegion = new Dictionary<string, int>();
            State.faultyDevices = new List<ThingInfo>();

            return base.OnActivateAsync();
        }

        public Task RegisterDevice(ThingInfo deviceInfo)
        {
            State.devices.Add(deviceInfo);
            return Task.FromResult(true);
        }

        public Task UnregisterDevice(ThingInfo deviceInfo)
        {
            State.devices.Remove(deviceInfo);
            return Task.FromResult(true);
        }

        public Task SendTelemetryAsync(ThingTelemetry telemetry)
        {
            if (telemetry.DevelopedFault)
            {
                if (false == State.faultsPerRegion.ContainsKey(telemetry.Region))
                {
                    State.faultsPerRegion[telemetry.Region] = 0;
                }
                State.faultsPerRegion[telemetry.Region]++;
                State.faultyDevices.Add(State.devices.Where(d => d.DeviceId == telemetry.DeviceId).FirstOrDefault());

                if (State.faultsPerRegion[telemetry.Region] > State.devices.Count(d => d.Region == telemetry.Region) / 3)
                {
                    Console.WriteLine("Sending an engineer to repair/replace devices in {0}", telemetry.Region);
                    foreach (var device in State.faultyDevices.Where(d => d.Region == telemetry.Region).ToList())
                    {
                        Console.WriteLine("\t{0}", device);
                    }
                }
            }

            return Task.FromResult(true);
        }
    }
}
