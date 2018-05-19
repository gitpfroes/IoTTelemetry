using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ThingGroupState : TableEntity
    {
        public List<ThingInfo> devices;
        public Dictionary<string, int> faultsPerRegion;
        public List<ThingInfo> faultyDevices;
    }
}
