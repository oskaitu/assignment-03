using Assignment3.Entities;
using var hello = new KanbanContext();

Console.WriteLine(hello.Database.CanConnect());
Console.WriteLine(hello.Database.GetDbConnection());
Console.WriteLine(hello.Database.GetConnectionString());
