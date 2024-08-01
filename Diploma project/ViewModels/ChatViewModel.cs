using Diploma_project.Models;
using System.Collections.Generic;

namespace Diploma_project.ViewModels
{
    public class ChatViewModel
    {
        public IEnumerable<Message> Messages { get; set; }

        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Message> ReadMessage { get; set; }

        public User SelectUser { get; set; } = new User();
    }
}
