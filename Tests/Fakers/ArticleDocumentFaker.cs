namespace Tests.Fakers;

public static class ArticleDocumentFaker
{
    private static int _Code = 1;

    private static readonly Faker<ArticleDocument> _ArticleDocumentFaker = new Faker<ArticleDocument>()
        .CustomInstantiator(f => new ArticleDocument { Id = Guid.NewGuid() })
        .RuleFor(u => u.Code, f => $"AC{(_Code++).ToString().PadLeft(6, '0')}")
        .RuleFor(u => u.Description, f => f.Commerce.ProductName());


    public static ArticleDocument Create() => _ArticleDocumentFaker.Generate();
}
