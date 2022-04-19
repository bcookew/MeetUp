using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetUp.Models
{
    public class LoginView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        // ====================== RegEx from https://www.ocpsoft.org/tutorials/regular-expressions/password-regular-expression/
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!#$%^&*<>()|+@\-_\[\]]).\S{8,32}$",ErrorMessage ="Password must be 8-32 chars including Nums, Upper, Lower, and Special Chars")]
        public string Password { get; set; }
    }
}