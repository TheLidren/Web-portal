using Diploma_project.App_Data;
using Diploma_project.Models;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    [Authorize(Roles = "programmer, direktor")]
    public class PositionController : Controller
    {
        readonly PortalContext db = new();
        readonly Regex trimmer = new(@"\s\s+");

        [HttpGet]
        public ActionResult CreatePosition() => View();

        [HttpPost]
        public ActionResult CreatePosition(Position position)
        {
            if (ModelState.IsValid)
            {
                Position pos = db.Positions.Where(m => m.Tittle.Contains(position.Tittle) && m.Status).FirstOrDefault();
                if (pos != null)
                {
                    ModelState.AddModelError("Tittle", "Данная должность существует с таким названием");
                    return View(position);
                }
                position.Tittle = trimmer.Replace(position.Tittle, " ").Trim();
                position.Status = true;
                db.Positions.Add(position);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(position);
        }

        [HttpGet]
        public ActionResult EditPosition(int? positionid)
        {
            Position Position = db.Positions.Find(positionid);
            if (positionid == null)
                return RedirectToAction("Error", "Home");
            return PartialView(Position);
        }

        [HttpPost]
        public ActionResult EditPosition(Position pos)
        {
            if (ModelState.IsValid)
            {
                Position position = db.Positions.Find(pos.Id);
                pos.Tittle = trimmer.Replace(pos.Tittle, " ").Trim();
                position.Tittle = pos.Tittle;
                db.Entry(position).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListUser", "Home");
            }
            return RedirectToAction("ListUser", "Home");
        }

        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeletePosition(int? positionid)
        {
            Position position = db.Positions.Find(positionid);
            if (positionid == null)
                return RedirectToAction("Error", "Home");
            position.Status = false;
            db.SaveChanges();
            return RedirectToAction("ListPosition");
        }
    }
}