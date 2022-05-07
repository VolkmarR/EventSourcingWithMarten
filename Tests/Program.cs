using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
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
            services.AddMarten(options =>
            {
                var connectionString = hostingContext.Configuration.GetConnectionString("Default");
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentException("Default connection string is not set in the configuration");

                options.Connection(connectionString);
            }); 

            services.AddTransient<Tests.UseCases.WorkingWithDocuments.UseCase>();

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
    await host.RunUseCase<Tests.UseCases.WorkingWithDocuments.UseCase>();

    Log.Information("Ending Test Run {timestamp}", DateTime.Now);
    // Shut down host
    lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}
