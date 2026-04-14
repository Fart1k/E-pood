using Microsoft.AspNetCore.Identity;

namespace Epood.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool isApprovedSeller { get; set; } = false;

    }
}
