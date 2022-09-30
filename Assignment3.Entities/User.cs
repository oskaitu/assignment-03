namespace Assignment3.Entities;

public class User
{
    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public int id { get; set; }

    [StringLength(50), Required]
    public string Name { get; set; }

    [StringLength(50), Required]
    public string Email { get; set; }

    public virtual ICollection<Task> tasks { get; set; }



}
