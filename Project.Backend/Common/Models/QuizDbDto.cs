using Project.Backend.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Backend.Common.Models;

public class QuizDbDto
{
    public int Id { get; set; }
    public bool Published { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
    public string? Description { get; set; }
}