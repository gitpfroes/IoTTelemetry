using Actors.Interfaces;
using Domain;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    class Thing : Actor, IThing
    {
        private ThingState State = new ThingState();
        public Thing(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            State.telemetry = new List<ThingTelemetry>();
            State.deviceGroupId = ""; // not activated
            return base.OnActivateAsync();
        }

        public Task SendTelemetryAsync(ThingTelemetry telemetry)
        {
            State.telemetry.Add(telemetry); // saving data at the device level
            if (State.deviceGroupId != "")
            {
                var deviceGroup = ActorProxy.Create<IThingGroup>(new ActorId(State.deviceGroupId));
                return deviceGroup.SendTelemetryAsync(telemetry); // sending telemetry data for aggregation
            }
            return Task.FromResult(true);
        }

        public Task ActivateMe(string region, int version)
        {
            State.deviceInfo = new ThingInfo()
            {
                DeviceId = Guid.NewGuid().ToString(),
                Region = region,
                Version = version
            };

            // based on the info, assign a group... for demonstration we are assigning a random group
            State.deviceGroupId = region;

            var deviceGroup = ActorProxy.Create<IThingGroup>(new ActorId(region));
            return deviceGroup.RegisterDevice(State.deviceInfo);
        }
    }
}

