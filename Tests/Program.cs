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

            services.AddTransient<DocumentTests>();

        })

        .Build();


using (host)
{
    await host.StartAsync();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    // do work here / get your work service ...
    await host.Services.GetRequiredService<DocumentTests>().ExecuteAsync();


    lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}



public class DocumentTests
{
    private readonly ILogger<DocumentTests> _Logger;
    private readonly IDocumentStore _Store;

    public DocumentTests(ILogger<DocumentTests> logger, IDocumentStore store)
    {
        _Logger = logger;
        _Store = store;
    }

    public async Task ExecuteAsync()
    {
        await InitAsync();
        await CreateArticleDocumentsAsync();
        await OutputCountAndFirst5DocumnentsAsync();
    }

    private async Task InitAsync()
    {
        _Logger.LogInformation("Deleting all Documents");

        // Clean out existing documents
        await _Store.Advanced.Clean.DeleteAllDocumentsAsync();
        _Logger.LogInformation("Documents deleting");
    }


    private async Task CreateArticleDocumentsAsync()
    {
        _Logger.LogInformation("Creating 100 ArticleDocuments");
        using var session = _Store.LightweightSession();

        for (int i = 0; i < 100; i++)
            session.Insert(Fakers.CreateArticleDocument());

        await session.SaveChangesAsync();
        _Logger.LogInformation("ArticleDocuments created");
    }

    private async Task OutputCountAndFirst5DocumnentsAsync()
    {
        using var session = _Store.QuerySession();

        var count = await session.Query<ArticleDocument>().CountAsync();
        _Logger.LogInformation("ArticleDocument Count: {count}", count);

        _Logger.LogInformation("First 5 ArticleDocuments");
        var firstFive = await session.Query<ArticleDocument>().Take(5).ToListAsync();
        foreach (var item in firstFive)
            _Logger.LogInformation("ID: {id}, Code: {code}, Description: {desc}", item.Id, item.Code, item.Description);

        _Logger.LogInformation("Load all ArticleDocuments");
        var all = await session.Query<ArticleDocument>().ToListAsync();
        _Logger.LogInformation("Loaded ArticleDocument Count: {count}", all.Count);

        _Logger.LogInformation("Filtering with Code");
        var spec = await session.Query<ArticleDocument>().SingleOrDefaultAsync(q => q.Code == "AC000005");
        _Logger.LogInformation("Item found: {code}", spec?.Code);

    }
}