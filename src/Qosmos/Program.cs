using Qosmos.Core.Network.Hosting;

var builder = QosmosApplication.CreateBuilder();

await builder
    .Build()
    .RunAsync();
