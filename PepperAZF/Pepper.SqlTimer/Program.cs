using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pepper.Service.Repositories;
using Pepper.Service.Repositories.Contracts;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((appBuilder, services) =>
    {
        var configuration = appBuilder.Configuration;
        services.AddScoped<IContactService, ContactService>();
    })
    .Build();
host.Run();