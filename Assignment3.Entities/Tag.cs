namespace Assignment3.Entities;

public class Tag
{
    public int id { get; set; }
    [StringLength(50), Required]
    public string Name { get; set; }

    public ICollection<Task> Tasks;

    public Tag(string name)
    {
        Name = name;
    }
}
