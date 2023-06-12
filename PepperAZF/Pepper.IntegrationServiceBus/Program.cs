
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pepper.DynamicsData;
using Pepper.DynamicsData.Interfaces;
using Pepper.DynamicsModels.Settings;
using Pepper.DynamicsService;
using Pepper.DynamicsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((appBuilder, services) =>
    {
        var configuration = appBuilder.Configuration;
        services.AddScoped<IDataverseConnectionService, DataverseConnectionService>();
        services.AddScoped<IDynamicsIntegrationService, DynamicsIntegrationService>();
        services.AddScoped<IContactDynamicsService, ContactDynamicsService>();
        services.AddScoped<IFieldDynamicsService, FieldDynamicsService>();

        services.AddOptions<DynamicsConfig>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("DynamicsConfig").Bind(settings);
            });
        services.AddOptions<ResponseMessagesAPI>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("ResponseMessagesAPI").Bind(settings);
            });
    })
    .Build();
host.Run();