using Diploma_project.EmailServices;
using Diploma_project.Models;
using Microsoft.AspNet.Identity.Owin;
using reCAPTCHA.MVC;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    [AllowAnonymous]
    public class DirektorController : Controller
    {
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }

        [HttpGet]
        public async Task<ActionResult> SubmitQuestion()
        {
            User user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (user != null)
            {
                ViewBag.Name = user.Name;
                ViewBag.Surname = user.Surname;
                ViewBag.Patronymic = user.Patronymic;
                ViewBag.Email = user.Email;
            }
            return View();
        }

        //Send message to direktor
        [HttpPost, CaptchaValidator]
        public ActionResult SubmitQuestion(Email email, HttpPostedFileBase[] uploadDocx)
        {
            if (ModelState.IsValid)
            {
                email.SendEmailAsync(email, uploadDocx);
                return RedirectToAction("SubmitQuestion");
            }
            return View(email);
        }
    }
}