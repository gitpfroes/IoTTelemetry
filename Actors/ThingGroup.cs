using Actors.Interfaces;
using Domain;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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

        private string connectionString = "DefaultEndpointsProtocol=https;AccountName=testepaola;AccountKey=DUFAWmh+e4y9pQsqeV+kuIfYlRG72RKmvoTCthsHUDHMjEemr3AEqqnK8YekdWvxS9bx45c2V8tJyWXXxo01cA==;EndpointSuffix=core.windows.net";
        private CloudStorageAccount cloudStorageAccount;
        private CloudTable cloudTable;

        public ThingGroup(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
            CloudStorageAccount.TryParse(connectionString, out cloudStorageAccount);
        }

        protected override Task OnActivateAsync()
        {
            State.devices = new List<ThingInfo>();
            State.faultsPerRegion = new Dictionary<string, int>();
            State.faultyDevices = new List<ThingInfo>();

            //Inicializa storage
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTable = cloudTableClient.GetTableReference("demotable");
            cloudTable.CreateIfNotExistsAsync();

            return base.OnActivateAsync();
        }

        public Task RegisterDevice(ThingInfo deviceInfo)
        {
            //Uploads device to cloud
            TableOperation insertOperation = TableOperation.InsertOrReplace(deviceInfo);
            cloudTable.ExecuteAsync(insertOperation);

            State.devices.Add(deviceInfo);
            return Task.FromResult(true);
        }

        public Task UnregisterDevice(ThingInfo deviceInfo)
        {
            State.devices.Remove(deviceInfo);
            TableOperation deleteOperation = TableOperation.Delete(deviceInfo);
            cloudTable.ExecuteAsync(deleteOperation);
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
