using System.Collections.ObjectModel;

namespace Assignment3.Entities;

public class TagRepository : ITagRepository
{

    private readonly KanbanContext _context;
    public TagRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = _context.Tags.FirstOrDefault(c => c.Name == tag.Name);
        if (entity is not null) return (Response.Conflict, entity.id);
        var newTag = new Tag(tag.Name);
        _context.Tags.Add(newTag);
        _context.SaveChanges();
        return (Response.Created, newTag.id);
    }
    public Response Delete(int tagId, bool force = false)
    {
        var deleteDis = _context.Tags.Include(c => c.Tasks).FirstOrDefault(c => c.id == tagId);
        if (deleteDis is null) return Response.NotFound;
        if (!deleteDis.Tasks.Any() || force)
        {
            _context.Tags.Remove(deleteDis);
            _context.SaveChanges();
            return Response.Deleted;

        }
        else return Response.Conflict;
    }

    public TagDTO Read(int tagId)
    {
        var tempTag = _context.Users.FirstOrDefault(c => c.id == tagId);
        if (tempTag is null) return null;
        return new TagDTO(tempTag.id, tempTag.Name);
    }

    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var returnList = new List<TagDTO>();

        foreach (var tag in _context.Tags)
        {
            returnList.Add(new TagDTO(tag.id, tag.Name));
        }

        return new ReadOnlyCollection<TagDTO>(returnList);
    }

    public Response Update(TagUpdateDTO tag)
    {
        var updateDis = _context.Users.Find(tag.Id);
        if (updateDis is null) return Response.NotFound;


        updateDis.Name = tag.Name;
        _context.Users.Update(updateDis);
        _context.SaveChanges();
        return Response.Updated;
    }
}
