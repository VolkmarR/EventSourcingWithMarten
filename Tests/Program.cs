using Serilog.Events;
using Tests.UseCases.EventSourcingTakeTwo;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Information)
    .WriteTo.File("TestRunLog.txt", outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true);
            config.AddUserSecrets<Program>();

            if (args != null)
            {
                config.AddCommandLine(args);
            }
        })
        .ConfigureServices((hostingContext, services) =>
        {
            var connectionString = hostingContext.Configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Default connection string is not set in the configuration");

            services.AddMarten(options =>
            {

                options.Connection(connectionString);
                options.AddSerilog();

                Tests.UseCases.SimpleEventsWithNoise.Register.ConfigureEmployees(options);
            });

            services.AddEventSourcingTakeTwoStore(connectionString);

            services.AddUseCases();

        })
        .UseSerilog()
        .Build();


using (host)
{
    // Start host
    await host.StartAsync();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
    Log.Information("Starting Test Run {timestamp}", DateTime.Now);

    // Run your UseCase here
    // await host.RunUseCase<Tests.UseCases.WorkingWithDocuments.UseCase>();

    // await host.RunUseCase<Tests.UseCases.SimpleEventsWithNoise.UseCase>();

    await host.RunUseCase<EventSourcingTakeTwo.UseCase>();


    Log.Information("Ending Test Run {timestamp}", DateTime.Now);
    // Shut down host
    lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}
