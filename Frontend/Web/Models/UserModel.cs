using System.ComponentModel.DataAnnotations;

namespace Pitka_Projekti_Front.Models;

public class UserModel
{
    [Display(Name = "Username")]
    public string? name { get; set; }

    [Display(Name = "Password")]
    public string? password { get; set; }

    [Display(Name = "Confirm Password")]
    public string? confirmPassword { get; set; }
}
