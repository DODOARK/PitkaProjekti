using System.ComponentModel.DataAnnotations;

namespace Pitka_Projekti_Front.Models;

public class TaskModel
{
    [Key]
    public int id { get; set; }

    [Display(Name = "Task Title")]
    public string? header { get; set; }

    [Display(Name = "Task Description")]
    public string? description { get; set; }

    [Display(Name = "Is completed?")]
    public bool done { get; set; }
}
