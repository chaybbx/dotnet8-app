using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class AppUser
{
    [Key]
    public int Id { get; set; } // because its int entity will auto increment the ID

    public required string UserName { get; set; }
}
