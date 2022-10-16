using Microsoft.AspNetCore.Identity;

namespace AlkemyChallenge.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}
