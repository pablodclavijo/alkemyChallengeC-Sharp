using System.ComponentModel.DataAnnotations;

namespace AlkemyChallenge.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

    }
}
