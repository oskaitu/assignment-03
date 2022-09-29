using System.Collections.ObjectModel;

namespace Assignment3.Entities.Tests;

public sealed class TaskRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;

    private readonly User? testUser1;

    private readonly User? testUser2;

    private readonly ICollection<Tag> allTags;

    public TaskRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>()
        .UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tags.AddRange(new Tag("hello") { id = 1 }, new Tag("KindaImportant") { id = 2 });
        context.Users.AddRange(new User("Oscar", "Oggelicious@svenskamail.dk") { id = 1 }, new User("Gotham", "nicolejs@tuborg.dk") { id = 2 });
        testUser1 = context.Users.FirstOrDefault(c => c.id == 1);
        testUser2 = context.Users.FirstOrDefault(c => c.id == 2);
        allTags = context.Tags.ToList();
        context.Tasks.AddRange(
            new Task("hello", testUser1, "Lets go ahead and do it", State.Active, allTags) { id = 1 },
         new Task("crazy important task", testUser2, "Long description", State.Removed, allTags.Where(b => b.Name.Equals("Hello")).ToList()) { id = 2 });
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);
    }


    [Fact]
    public void Create_ConfirmThatTaskHasBeenCreated_ByResponseAndByID()
    {
        // Given
        var (response, created) = _repository.Create(new TaskCreateDTO("Water the garden", testUser1.id, "", new Collection<String> { "Very important", "dontforget" }));

        response.Should().Be(Response.Created);

        created.Should().Be(3);

    }

    [Fact]
    public void read_CallMethodWithTestDb_GetCollectionOfAllTestPersons()
    {
        // Given

        var suspectedTasks = new List<TaskDTO> {
            new TaskDTO(1, "hello", testUser1.Name, new Collection<String> { "Very important", "dontforget" }, State.Active),
        new TaskDTO(1, "dont go there", testUser1.Name, new Collection<String> { "Very important", "dontforget" }, State.Removed)};
        // When
        var allTasks = _repository.ReadAll();

        // Then
        allTasks.Should().BeEquivalentTo(suspectedTasks);
    }


    [Fact]
    public void read_GivenId1_shouldBeTag_KindaImportant()
    {
        // Given
        var suspectedTasks = new TaskDTO(2, "hello", testUser1.Name, (Collection<String>)allTags, State.Active);

        // When
        var outputTask = _repository.Read(2);
        // Then
        outputTask.Should().Be(suspectedTasks);
    }

    [Fact]
    public void Update_shouldReturnName_TotallyImportant_forHelloChangeToHelloButNowUpdated_AndResponseIs_Updated()
    {
        // Given
        var originalTag = _repository.Read(1);
        // When
        var response = _repository.Update(new TaskUpdateDTO(2, "hello but now updated", testUser1.id, "", (Collection<String>)allTags, State.Active));
        var updatedTag = _repository.Read(1);
        // Then
        response.Should().Be(Response.Updated);
        originalTag.Should().NotBe(updatedTag);
        updatedTag.Should().BeEquivalentTo(2, "hello but now updated", testUser1.id, "", (Collection<String>)allTags, State.Active);
    }


    [Fact]
    public void Delete_deleteShouldReturnResponseDeleted_ReadingId1ShouldReturnStateRemoved_ForDeletingTaskWithID1()
    {
        var response = _repository.Delete(1);
        response.Should().Be(Response.Deleted);
        var deletedTask = _repository.Read(1);
        deletedTask.State.Should().Be(State.Removed);

    }


    public void Dispose()
    {
        _context.Dispose();
    }


}
