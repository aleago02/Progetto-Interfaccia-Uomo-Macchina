using System.ComponentModel.DataAnnotations;

namespace Template.Web.Features.Login
{
    public class LoginViewModel
    {
        public LoginViewModel() { }
        [Required]
        [Display(Name = "Email")]
        //[DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Rimani connesso")]
        public bool RememberMe { get; set; }

    }
}
