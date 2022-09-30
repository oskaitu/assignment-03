using Microsoft.Data.Sqlite;

namespace Ass3.New.Tests;

public sealed class UserRepositoryTests : IDisposable
{

    private readonly KanbanContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>()
        .UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Users.AddRange(new User("Oscar", "Oggelicious@svenskamail.dk") { id = 1 }, new User("Gotham", "nicolejs@tuborg.dk") { id = 2 });
        //context.Tasks.Add(new Task { id = 1, AlterEgo = "Superman", CityId = 1 });
        context.SaveChanges();

        _context = context;
        _repository = new UserRepository(_context);
    }


    [Fact]
    public void Create_ConfirmThatUserHasBeenCreated_ByResponseAndByID()
    {
        // Given
        var (response, created) = _repository.Create(new UserCreateDTO("Henrik", "osca@osca.dk"));

        response.Should().Be(Response.Created);

        created.Should().Be(3);

    }
    [Fact]
    public void read_CallMethodWithTestDb_GetCollectionOfAllTestPersons()
    {
        // Given

        var suspectedUsers = new List<UserDTO> { new UserDTO(1, "Oscar", "Oggelicious@svenskamail.dk"), new UserDTO(2, "Gotham", "nicolejs@tuborg.dk") };
        // When
        var allUsers = _repository.ReadAll();

        // Then
        allUsers.Should().BeEquivalentTo(suspectedUsers);
    }

    [Fact]
    public void read_GivenId1_shouldBeOscarWithEmail()
    {
        // Given

        var suspectedUser = new UserDTO(1, "Oscar", "Oggelicious@svenskamail.dk");

        // When
        var outputUser = _repository.Read(1);
        // Then
        outputUser.Should().Be(suspectedUser);
    }

    [Fact]
    public void Update_shouldReturnNameErik_forOscarNameChangeToErikAndResponse_Updated()
    {
        // Given
        var originalUser = _repository.Read(1);
        // When
        var response = _repository.Update(new UserUpdateDTO(1, "Erik", "Oggelicious@svenskamail.dk"));
        var updatedUser = _repository.Read(1);
        // Then
        response.Should().Be(Response.Updated);
        originalUser.Should().NotBe(updatedUser);
        updatedUser.Should().BeEquivalentTo(new UserDTO(1, "Erik", "Oggelicious@svenskamail.dk"));
    }


    [Fact]
    public void Delete_deleteShouldReturnResponseDeleted_ReadingId1ShouldReturnNull_ForDeletingID1()
    {
        var response = _repository.Delete(1, false);
        response.Should().Be(Response.Deleted);
        var deletedUser = _repository.Read(1);
        deletedUser.Should().BeNull();

    }


    public void Dispose()
    {
        _context.Dispose();
    }
}
