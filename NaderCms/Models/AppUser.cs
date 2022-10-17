using Microsoft.AspNetCore.Identity;

namespace NaderCms.Models
{
    public class AppUser : IdentityUser
    {
        public string Occupation { get; set; }
    }
}
