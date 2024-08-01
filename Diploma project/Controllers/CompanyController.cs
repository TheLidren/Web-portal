using Diploma_project.App_Data;
using Diploma_project.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        readonly PortalContext db = new();
        readonly Regex trimmerspace = new(@"\s\s+");
        User user;
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }

        public Services RegexServices(Services services)
        {
            services.Tittle = trimmerspace.Replace(services.Tittle, " ").Trim();
            services.ListServices = Regex.Replace(services.ListServices, @"\r\n+", "\n", RegexOptions.Multiline);
            services.ListServices = Regex.Replace(services.ListServices, @"\n+", "\r\n", RegexOptions.Multiline);
            return services;
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Contact(SortStateContact sortOrder = SortStateContact.SurnameAsc)
        {
            IQueryable<ContactInformation> contacts = db.Contacts.Include(x => x.Position);

            ViewData["SurnameSort"] = sortOrder == SortStateContact.SurnameAsc ? SortStateContact.SurnameDesc : SortStateContact.SurnameAsc;
            ViewData["PositionSort"] = sortOrder == SortStateContact.PositionAsc ? SortStateContact.PositionDesc : SortStateContact.PositionAsc;
            contacts = sortOrder switch
            {
                SortStateContact.SurnameDesc => contacts.OrderByDescending(s => s.Surname),
                SortStateContact.PositionAsc => contacts.OrderBy(s => s.Position.Tittle),
                SortStateContact.PositionDesc => contacts.OrderByDescending(s => s.Position.Tittle),
                _ => contacts.OrderBy(s => s.Surname),
            };
            return View(contacts.AsNoTracking().ToList());
        }

        [HttpGet]
        [Authorize(Roles = "department, programmer, direktor")]
        public ActionResult AddContact()
        {
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle");
            ViewBag.positions = positions;
            return View();
        }

        //Add contact in table "ContactInformation"
        [HttpPost]
        [Authorize(Roles = "department, programmer, direktor")]
        public ActionResult AddContact(ContactInformation contact)
        {
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", contact.PositionId);
            ViewBag.positions = positions;
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Contact");
            }
            return View(contact);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult AboutCompany() => View(db.Services.Include(u => u.User).ToList());

        [HttpGet]
        [Authorize(Roles = "programmer, direktor, section manager")]
        public ActionResult AddServices() => View();

        //Add services in table "Services"
        [HttpPost]
        [Authorize(Roles = "programmer, direktor, section manager")]
        public async Task<ActionResult> AddServices(Services services)
        {
            if (ModelState.IsValid)
            {
                services = RegexServices(services);
                services.ListServices = services.ListServices.Replace("\r\n", "</li><li style='text-indent:1em; '>");
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                services.UserId = user.Id;
                db.Services.Add(services);
                db.SaveChanges();
                return RedirectToAction("AboutCompany");
            }
            return View(services);
        }

        [HttpGet]
        [Authorize(Roles = "department, programmer, direktor")]
        public ActionResult EditContact(int? id)
        {
            ContactInformation contact = db.Contacts.Find(id);
            if (contact == null)
                return RedirectToAction("Error", "Home");
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", contact.PositionId);
            ViewBag.positions = positions;
            return View(contact);
        }

        //Edit contact in table "ContactInformation"
        [HttpPost]
        [Authorize(Roles = "department, programmer, direktor")]
        public ActionResult EditContact(ContactInformation contact)
        {
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", contact.PositionId);
            ViewBag.positions = positions;
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Contact");
            }
            return View(contact);
        }

        //Delete services in table "Services"
        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeleteServices(int? id)
        {
            Services services = db.Services.Find(id);
            if (services == null)
                return RedirectToAction("Error", "Home");
            db.Services.Remove(services);
            db.SaveChanges();
            return RedirectToAction("AboutCompany");
        }

        //Delete contact in table "ContactInformation"
        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeleteContact(int? id)
        {
            ContactInformation contact = db.Contacts.Find(id);
            if (contact == null)
                return RedirectToAction("Error", "Home");
            db.Contacts.Remove(contact);
            db.SaveChanges();
            return RedirectToAction("Contact");
        }

        [HttpGet]
        [Authorize(Roles = "programmer, direktor, section manager")]
        public ActionResult EditServices(int? id)
        {
            Services services = db.Services.Find(id);
            if (services == null)
                return RedirectToAction("Error", "Home");
            services.ListServices = services.ListServices.Replace("</li><li style='text-indent:1em; '>", "\r\n");
            return View(services);
        }

        //Edit services in table "Services"
        [HttpPost]
        [Authorize(Roles = "programmer, direktor, section manager")]
        public async Task<ActionResult> EditServices(Services services)
        {
            if (ModelState.IsValid)
            {
                services = RegexServices(services);
                services.ListServices = services.ListServices.Replace("\r\n", "</li><li style='text-indent:1em; '>");
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                services.UserId = user.Id;
                db.Entry(services).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AboutCompany");
            }
            return View(services);
        }
    }
}
