using System;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Domain
{
    public class ThingInfo : TableEntity
    {
        public string DeviceId { get; set; }
        public string Region { get; set; }
        public int Version { get; set; }
    }
}
