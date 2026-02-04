using Qosmos.Core.Network.Hosting;

var builder = QosmosApplication.CreateBuilder(args);

await builder
    .Build()
    .RunAsync();
