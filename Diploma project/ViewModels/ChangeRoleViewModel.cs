using Diploma_project.Models;
using System.Collections.Generic;

namespace Diploma_project.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public IEnumerable<Role> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }

    }
}
