using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Actors.Interfaces
{
    public interface IThing : IActor
    {
        Task ActivateMe(string region, int version);
        Task SendTelemetryAsync(ThingTelemetry telemetry);
    }
}
