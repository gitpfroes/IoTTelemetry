using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ThingState
    {
        public List<ThingTelemetry> telemetry;
        public ThingInfo deviceInfo;
        public string deviceGroupId;
    }
}
