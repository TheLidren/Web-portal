using Diploma_project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;

namespace Diploma_project.App_Data
{
    public class AppDbInitializer : DropCreateDatabaseAlways<PortalContext>
    {
        readonly PortalContext db = new();
        protected override void Seed(PortalContext context)
        {
            var roleManager = new RoleManager<Role>(new RoleStore<Role>(context));
            roleManager.Create(new Role { Name = "direktor", Description = "Директор обладает всеми правами" });
            roleManager.Create(new Role { Name = "programmer", Description = "Программист обладает всеми правами" });
            roleManager.Create(new Role { Name = "department", Description = "Отдел кадров отвечает за опубликованные контакты" });
            roleManager.Create(new Role { Name = "secretary", Description = "Секретарь отвечает за события и мероприятия, проходящие в организации" });
            roleManager.Create(new Role { Name = "section manager", Description = "Начальник участка отвечает за оказываемые работы и услуги в организации" });
            roleManager.Create(new Role { Name = "user", Description = "Рядовой пользователь. Имеет стандартный уровень доступа" });
            roleManager.Create(new Role { Name = "direktor", Description = "Директор обладает всеми правами" });
            IEnumerable<Position> positions = new[]
            {
                new Position { Tittle = "Программист", Status = true },
                new Position { Tittle = "Отдел кадров", Status = true },
                new Position { Tittle = "Начальник участка", Status = true },
                new Position { Tittle = "Секретарь", Status = true },
                new Position { Tittle = "Директор", Status = false }
            };
            db.Positions.AddRange(positions);
            db.SaveChanges();
            base.Seed(context);
        }
    }
}
