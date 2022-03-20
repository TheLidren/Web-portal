using Microsoft.AspNet.Identity.EntityFramework;

namespace Diploma_project.Models
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
