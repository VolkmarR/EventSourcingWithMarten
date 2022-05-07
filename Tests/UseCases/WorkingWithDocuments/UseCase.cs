namespace Tests.UseCases.WorkingWithDocuments;

internal class UseCase : IUseCase
{
    private readonly IDocumentStore _Store;

    public UseCase(IDocumentStore store) => _Store = store;

    public async Task ExecuteAsync()
    {
        await InitAsync();
        await CreateArticleDocumentsAsync();
        await OutputCountAndFirst5DocumnentsAsync();
    }

    private async Task InitAsync()
    {
        // Clean out existing documents
        using var _ = Operation.Time("Deleting all Documents");
        await _Store.Advanced.Clean.DeleteAllDocumentsAsync();
    }

    private async Task CreateArticleDocumentsAsync()
    {
        using var _ = Operation.Time("Creating 100 ArticleDocuments");
        using var session = _Store.LightweightSession();

        for (int i = 0; i < 100; i++)
            session.Insert(ArticleDocumentFaker.Create());

        await session.SaveChangesAsync();
    }

    private async Task OutputCountAndFirst5DocumnentsAsync()
    {
        using var session = _Store.QuerySession();
        using var _ = Operation.Time("Querring Documents");

        var count = await session.Query<ArticleDocument>().CountAsync();
        Log.Information("ArticleDocument Count: {count}", count);

        Log.Information("First 5 ArticleDocuments");
        var firstFive = await session.Query<ArticleDocument>().Take(5).ToListAsync();
        foreach (var item in firstFive)
            Log.Information("ID: {id}, Code: {code}, Description: {desc}", item.Id, item.Code, item.Description);

        Log.Information("Load all ArticleDocuments");
        var all = await session.Query<ArticleDocument>().ToListAsync();
        Log.Information("Loaded ArticleDocument Count: {count}", all.Count);

        Log.Information("Filtering with Code");
        var spec = await session.Query<ArticleDocument>().SingleOrDefaultAsync(q => q.Code == "AC000005");
        Log.Information("Item found: {code}", spec?.Code);

    }
}
