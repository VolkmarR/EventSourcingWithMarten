namespace Tests.UseCases;

internal class WorkingWithDocuments : IUseCase
{
    private readonly ILogger<WorkingWithDocuments> _Logger;
    private readonly IDocumentStore _Store;

    public WorkingWithDocuments(ILogger<WorkingWithDocuments> logger, IDocumentStore store)
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
            session.Insert(ArticleDocumentFaker.Create());

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
