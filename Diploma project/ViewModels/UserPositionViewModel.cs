using Diploma_project.Models;
using System.Collections.Generic;

namespace Diploma_project.ViewModels
{
    public class UserPositionViewModel
    {
        public IEnumerable<User> Users { get; set; }

        public IEnumerable<Position> Positions { get; set; }

    }
}
