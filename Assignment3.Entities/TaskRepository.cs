namespace Assignment3.Entities;

public class TaskRepository : ITaskRepository
{
    private readonly KanbanContext _context;
    public TaskRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {
        // Get the assigned user
        var assignedUser = _context.Users.FirstOrDefault(c => c.id == task.AssignedToId);

        if (assignedUser == null) return (Response.BadRequest, (int)task.AssignedToId);

        List<Tag> assignedTags = new();

        // Create the associated tags
        foreach (String tag in task.Tags)
        {
            var tagToAdd = _context.Tags.FirstOrDefault(c => c.Name == tag);

            if (tagToAdd is null) _context.Tags.Add(new Tag(tag));

            assignedTags.Add(tagToAdd);

        }

        var newTask = new Task(task.Title, assignedUser, task.Description, State.New, assignedTags);

        _context.Tasks.Add(newTask);
        _context.SaveChanges();
        return (Response.Created, newTask.id);
    }

    public Response Delete(int taskId)
    {
        var deleteDis = _context.Tasks.FirstOrDefault(c => c.id == taskId);
        if (deleteDis is null) return Response.NotFound;
        if (deleteDis.state == State.New)
        {
            _context.Tasks.Remove(deleteDis);
            _context.SaveChanges();
            return Response.Deleted;
        }
        if (deleteDis.state == State.Active)
        {
            var stringListOfTags = new List<string>();
            foreach (var tag in deleteDis.tags)
            {
                stringListOfTags.Append(tag.Name);
            }

            Update(new TaskUpdateDTO(taskId, deleteDis.Title, deleteDis.AssignedTo.id, deleteDis.Description, stringListOfTags
            , State.Removed));

            return Response.Deleted;

        }
        return Response.Conflict;


    }

    public TaskDetailsDTO Read(int taskId)
    {

        var tempTask = _context.Tasks.FirstOrDefault(c => c.id == taskId);
        List<String> listOfTags = new();

        foreach (var tag in tempTask.tags)
        {
            listOfTags.Add(tag.Name);
        }


        DateTime DateTime = new();

        if (tempTask is null) return null;

        return new TaskDetailsDTO(tempTask.id, tempTask.Title, tempTask.Description, DateTime, tempTask.AssignedTo.Name, listOfTags, State.Active, DateTime);
    }



    public IReadOnlyCollection<TaskDTO> ReadAll()
    {
        var returnList = new List<TaskDTO>();

        foreach (var user in _context.Tasks)
        {
            var tags = new List<String>();
            foreach (var tag in user.tags)
            {
                tags.Add(tag.Name);

            }

            returnList.Add(new TaskDTO(user.id, user.Title, user.AssignedTo.Name, tags, user.state));
        }

        return new System.Collections.ObjectModel.ReadOnlyCollection<TaskDTO>(returnList);
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
    {
        return (IReadOnlyCollection<TaskDTO>)ReadAll().Where(c => c.State == state);
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
    {
        return (IReadOnlyCollection<TaskDTO>)ReadAll().Where(c => c.Tags.Equals(tag));
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
    {
        return (IReadOnlyCollection<TaskDTO>)ReadAll().Where(c => c.AssignedToName == _context.Users.FirstOrDefault(c => c.id == userId).Name);
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
    {
        return ReadAllByTag("Removed");
    }

    public Response Update(TaskUpdateDTO task)
    {
        var updateDis = _context.Tasks.FirstOrDefault(c => c.id == task.Id);
        if (updateDis is null) return Response.NotFound;
        updateDis.Title = task.Title;
        updateDis.AssignedTo = _context.Users.FirstOrDefault(c => c.id == task.AssignedToId);
        updateDis.Description = task.Description;
        updateDis.state = task.State;
        List<Tag> newTags = new();

        // Create the associated tags
        foreach (String tag in task.Tags)
        {
            var tagToAdd = _context.Tags.FirstOrDefault(c => c.Name == tag);

            if (tagToAdd is null) _context.Tags.Add(new Tag(tag));

            newTags.Add(tagToAdd);

        }

        updateDis.tags = newTags;


        _context.Tasks.Update(updateDis);
        _context.SaveChanges();
        return Response.Updated;
    }
}
