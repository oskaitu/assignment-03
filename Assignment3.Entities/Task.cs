namespace Assignment3.Entities;



public class Task
{
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    public User? AssignedTo { get; set; }

    [StringLength(int.MaxValue)]
    public string? Description { get; set; }

    [Required]
    public State state { get; set; }

    // Implement 
    public virtual ICollection<Tag> tags { get; set; }

    public Task()
    {
        tags = new List<Tag>();
    }

    public Task(string title, User? assignedUser, string? description, State state, ICollection<Tag> tags)
    {
        Title = title;
        this.AssignedTo = assignedUser;
        this.Description = description;
        this.state = state;
        this.tags = tags;
    }
}
