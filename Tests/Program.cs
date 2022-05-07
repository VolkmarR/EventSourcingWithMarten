using Tests.UseCases;

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
            services.AddLogging(opt =>
            {
                opt.AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss.fff] ");
            });

            services.AddMarten(options =>
            {
                var connectionString = hostingContext.Configuration.GetConnectionString("Default");
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentException("Default connection string is not set in the configuration");

                options.Connection(connectionString);
            }).InitializeStore(); 

            services.AddTransient<WorkingWithDocuments>();

        })

        .Build();


using (host)
{
    // Start host
    await host.StartAsync();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    // Run your UseCase here
    await host.RunUseCase<WorkingWithDocuments>();

    // Shut down host
    lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}
