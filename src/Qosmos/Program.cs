using Qosmos.Core.Network.Hosting;

var builder = QosmosApplication.CreateBuilder();

await using var app = builder.Build();

await app.RunAsync();
