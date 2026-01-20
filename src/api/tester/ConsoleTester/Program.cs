using System.Reflection;
using Carter;
using Finbuckle.MultiTenant.Abstractions;
using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.Booking.Application;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Infrastructure;
using FSH.Starter.WebApi.Booking.Infrastructure.Persistence;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);



        var assemblies = new Assembly[]
                {
            //typeof(CatalogMetadata).Assembly,
            //typeof(TodoModule).Assembly,
            typeof(BookingMetadata).Assembly
                };

        builder.Services.AddLogging(configure => configure.AddConsole());

        //register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        //register mediatr
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        //register module services
        //builder.RegisterCatalogServices();
        //builder.RegisterTodoServices();
        builder.RegisterBookingServices();

        //add carter endpoint modules
        builder.Services.AddCarter(configurator: config =>
        {
            //config.WithModule<CatalogModule.Endpoints>();
            //config.WithModule<TodoModule.Endpoints>();
            config.WithModule<BookingModule.Endpoints>();
        });

        builder.Services.AddSingleton<MyApp>();

        var host = builder.Build();

        //host.Run();

        await host.Services.GetRequiredService<MyApp>().StartAsync();
        Console.WriteLine("Done!");
    }
}

class MyApp
{
    private readonly IRepository<Customer> _customerRepository;
    public MyApp([FromKeyedServices("booking:customers")] IRepository<Customer> repository)
    {
        _customerRepository = repository;
    }

    public Task StartAsync()
    {
        Console.WriteLine("Hello World!");
        _customerRepository.AddAsync(Customer.Create("Test User", "1234567890", "123 Test St", "No notes", "", "555-1234", "00000"));
        _customerRepository.SaveChangesAsync();
        return Task.CompletedTask;
    }
}


