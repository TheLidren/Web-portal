using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Diploma_project.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Messages = new HashSet<Message>();
        }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Password { get; set; }

        public int? PositionId { get; set; }

        public Position Position { get; set; }

        public bool Status { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<FilesGalery> FilesGaleries { get; set; }

        public virtual ICollection<Services> Services { get; set; }

        public virtual ICollection<FilesDocuments> FilesDocuments { get; set; }

        public string ReceiveUserId (string name, UserManager<User> manager)
        {
            User user = manager.FindByEmail(name);
            return user.Id;
        }

    }
}
