namespace Assignment3.Entities.Tests;

public sealed class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>()
        .UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tags.AddRange(new Tag("hello") { id = 1 }, new Tag("KindaImportant") { id = 2 });
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public void Create_ConfirmThatTagHasBeenCreated_ByResponseAndByID()
    {
        // Given
        var (response, created) = _repository.Create(new TagCreateDTO("DontForget"));

        response.Should().Be(Response.Created);

        created.Should().Be(3);

    }


    [Fact]
    public void read_CallMethodWithTestDb_GetCollectionOfAllTestTags()
    {
        // Given

        var suspectedTags = new List<TagDTO> { new TagDTO(1, "hello"), new TagDTO(2, "KindaImportant") };
        // When
        var allTags = _repository.ReadAll();

        // Then
        allTags.Should().BeEquivalentTo(suspectedTags);
    }

    [Fact]
    public void read_GivenId1_shouldBeTag_KindaImportant()
    {
        // Given

        var suspectedTag = new TagDTO(2, "KindaImportant");

        // When
        var outputTag = _repository.Read(1);
        // Then
        outputTag.Should().Be(suspectedTag);
    }

    [Fact]
    public void Update_shouldReturnName_TotallyImportant_forHelloNameChangeToTotallyImportant_AndResponseIs_Updated()
    {
        // Given
        var originalTag = _repository.Read(1);
        // When
        var response = _repository.Update(new TagUpdateDTO(1, "TotallyImportant"));
        var updatedTag = _repository.Read(1);
        // Then
        response.Should().Be(Response.Updated);
        originalTag.Should().NotBe(updatedTag);
        updatedTag.Should().BeEquivalentTo(new TagDTO(1, "TotallyImportant"));
    }

    [Fact]
    public void Delete_deleteShouldReturnResponseDeleted_ReadingId1ShouldReturnNull_ForDeletingTagWithID1()
    {
        var response = _repository.Delete(1, false);
        response.Should().Be(Response.Deleted);
        var deletedTag = _repository.Read(1);
        deletedTag.Should().BeNull();

    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
