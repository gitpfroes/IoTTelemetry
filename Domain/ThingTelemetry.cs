using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ThingTelemetry : TableEntity
    {
        public bool DevelopedFault { get; set; }
        public string Region { get; set; }
        public string DeviceId { get; set; }
    }
}
