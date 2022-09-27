namespace Assignment3.Entities.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment3.Core;

public sealed class TaskRepository : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests(){
        
        var connection = new SqliteConnection("Filename:memory:");
        connection.Open();

    }

}


