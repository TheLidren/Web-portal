using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace Diploma_project.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message) =>
            await Clients.All.SendAsync("receiveMessage", message);
    }
}
