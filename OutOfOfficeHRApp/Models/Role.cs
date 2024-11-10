using Microsoft.AspNetCore.Identity;

namespace OutOfOfficeHRApp.Models
{
    public class Role : IdentityRole
    {
        public Role() : base() { }

        public Role(string name) : base(name)
        {
        }
    }
}
