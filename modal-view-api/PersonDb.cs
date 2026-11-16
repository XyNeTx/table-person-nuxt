using System;
using Microsoft.EntityFrameworkCore;
using modal_view_api.Models;

namespace modal_view_api;

public class PersonDb : DbContext
{
    public PersonDb(DbContextOptions<PersonDb> options) : base(options)
    {
        
    }

    public DbSet<Person> Person { get; set; }
}
