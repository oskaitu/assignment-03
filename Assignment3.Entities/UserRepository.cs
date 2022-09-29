using System.Collections.ObjectModel;

namespace Assignment3.Entities;

public sealed class UserRepository : IUserRepository
{
    private readonly KanbanContext _context;
    public UserRepository(KanbanContext context)
    {
        _context = context;
    }


    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(c => c.Email == user.Email);
        if (entity is not null) return (Response.Conflict, entity.id);
        var newUser = new User(user.Name, user.Email);
        _context.Users.Add(newUser);
        _context.SaveChanges();
        return (Response.Created, newUser.id);
    }

    public IReadOnlyCollection<UserDTO> ReadAll()
    {

        var returnList = new List<UserDTO>();

        foreach (var user in _context.Users)
        {
            returnList.Add(new UserDTO(user.id, user.Name, user.Email));
        }

        return new ReadOnlyCollection<UserDTO>(returnList);
    }

    public UserDTO Read(int userId)
    {
        var tempUser = _context.Users.FirstOrDefault(c => c.id == userId);
        if (tempUser is null) return null;
        return new UserDTO(tempUser.id, tempUser.Name, tempUser.Email);
    }

    public Response Update(UserUpdateDTO user)
    {
        var updateDis = _context.Users.Find(user.Id);
        if (updateDis is null) return Response.NotFound;


        updateDis.Name = user.Name;
        updateDis.Email = user.Email;
        _context.Users.Update(updateDis);
        _context.SaveChanges();
        return Response.Updated;
    }

    public Response Delete(int userId, bool force)
    {
        var deleteDis = _context.Users.Include(c => c.tasks).FirstOrDefault(c => c.id == userId);
        if (deleteDis is null) return Response.NotFound;
        if (!deleteDis.tasks.Any() || force)
        {
            _context.Users.Remove(deleteDis);
            _context.SaveChanges();
            return Response.Deleted;

        }
        else return Response.Conflict;
    }
}
