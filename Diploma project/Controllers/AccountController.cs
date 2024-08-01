using Diploma_project.App_Data;
using Diploma_project.Models;
using Diploma_project.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        readonly PortalContext db = new();
        User user;
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        private IAuthenticationManager AuthenticationManager { get => HttpContext.GetOwinContext().Authentication; }
        private ApplicationRoleManager RoleManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>(); }

        [HttpGet, AllowAnonymous]
        public ActionResult Login() => PartialView();
        
        [HttpGet]
        public ActionResult ListUsers() => View(db.Users.ToList());

        //User autorization 
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                user = await UserManager.FindAsync(model.Email, model.Password);
                if (user == null || !user.Status)
                    return RedirectToAction("Index", "Home");
                if (user.EmailConfirmed)
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, 
                        DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                }
                else
                    return RedirectToAction("Confirm", "Account", new { user.Email });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Register()
        {
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle");
            ViewBag.positions = positions;
            return View();
        }

        //User registration
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", registerViewModel.PositionId);
            ViewBag.positions = positions;
            if (ModelState.IsValid)
            {
                user = new(){ 
                    UserName = registerViewModel.Email,Name = registerViewModel.Name, Surname = registerViewModel.Surname,
                    Patronymic = registerViewModel.Patronymic, Email = registerViewModel.Email,
                    PositionId = registerViewModel.PositionId, Password = registerViewModel.Password,
                    Status = true};
                var result = await UserManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "user");
                    string message = string.Format("Для завершения регистрации перейдите по ссылке:" +
                            "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                Url.Action("ConfirmEmail", "Account", new { userId = user.Id, user.Email }, Request.Url.Scheme));
                    EmailServices.Email.ComfirmEmailMessage(user.Email, message, "Подтвердите регистрацию");
                    return RedirectToAction("Confirm", "Account");
                }
                else
                    ModelState.AddModelError("Email", "Данный email уже зарегестрирован");
            }
            return View(registerViewModel);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Confirm() => View();

        //Confirm account
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string email)
        {
            user = await UserManager.FindByIdAsync(userId);
            if (user == null || email == null)
                return RedirectToAction("Error", "Home");
            else if (user.Email == email)
            {
                user.EmailConfirmed = true;
                await UserManager.UpdateAsync(user);
                ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Confirm", "Account");
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            user = UserManager.FindByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Error", "Home");
            SelectList positions;
                if(User.IsInRole("direktor"))
                positions = new(db.Positions, "Id", "Tittle", user.PositionId);
            else
                positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", user.PositionId);
            ViewBag.positions = positions;
            EditAccountViewModel editAccount = new() {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Patronymic = user.Patronymic,
                PositionId = user.PositionId,
            };
            EditPasswordViewModel editPassword = new()
            {
                Id = user.Id,
                OldPassword = user.Password
            };
            AccountViewModel viewModel = new()
            {
                EditAccountView = editAccount,
                EditPasswordView = editPassword,
                EditEmailView = new() { Id = user.Id, Email = user.Email}
            };
            return View(viewModel);
        }

        //Change user information
        [HttpPost]
        public ActionResult EditAccount(AccountViewModel model)
        {
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", model.EditAccountView.PositionId);
            ViewBag.positions = positions;
            if (ModelState.IsValid)
            {
                user = UserManager.FindById(model.EditAccountView.Id);
                if (user == null)
                    return RedirectToAction("Error", "Home");
                user.Name = model.EditAccountView.Name;
                user.Surname = model.EditAccountView.Surname;
                user.Patronymic = model.EditAccountView.Patronymic;
                user.PositionId = model.EditAccountView.PositionId;
                var result = UserManager.Update(user);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        //Change password
        [HttpPost]
        public async Task<ActionResult> EditPassword(AccountViewModel model)
        {
            user = await UserManager.FindByIdAsync(model.EditPasswordView.Id);
            if (user == null)
                return RedirectToAction("Error", "Home");
            model.EditAccountView = new(){
                Id = user.Id, Name = user.Name,
                Surname = user.Surname, Patronymic = user.Patronymic,
                PositionId = user.PositionId};
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", 
                model.EditAccountView.PositionId);
            ViewBag.positions = positions;
            if (ModelState.IsValid)
            {
                IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, 
                    model.EditPasswordView.OldPassword, model.EditPasswordView.NewPassword);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }
            return View("EditAccount", model);
        }

        //Change email
        [HttpPost]
        public async Task<ActionResult> EditEmail(AccountViewModel model)
        {
            user = await UserManager.FindByIdAsync(model.EditEmailView.Id);
            if (user == null)
                return RedirectToAction("Error", "Home");
            model.EditAccountView = new(){
                Id = user.Id, Name = user.Name,
                Surname = user.Surname, Patronymic = user.Patronymic,
                PositionId = user.PositionId};
            SelectList positions = new(db.Positions.Where(u => u.Status), "Id", "Tittle", model.EditAccountView.PositionId);
            ViewBag.positions = positions;
            User checkUser = await UserManager.FindByEmailAsync(model.EditEmailView.Email);
            if (checkUser != null){
                ModelState.AddModelError("EditEmailView.Email", "Данный email уже зарегестрирован");
                return View("EditAccount", model);}
            if (ModelState.IsValid)
            {
                user.Email = model.EditEmailView.Email;
                user.UserName = model.EditEmailView.Email;
                user.EmailConfirmed = false;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded){
                    string message = string.Format("Для смены почты перейдите по ссылке:" +
                    "<a href=\"{0}\" title=\"Подтвердить почту\">{0}</a>",
                    Url.Action("ConfirmEmail", "Account", new { userId = user.Id, user.Email }, Request.Url.Scheme));
                    EmailServices.Email.ComfirmEmailMessage(user.Email, message, "Смена почты аккаунта");
                    return RedirectToAction("Confirm", "Account", new { user.Email });}
            }
            ModelState.AddModelError("Email", "Данный email уже зарегестрирован");
            return View("EditAccount", model);
        }

        //Delete user account
        [HttpGet]
        public async Task<ActionResult> DeleteAccount()
        {
            user = await UserManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Error", "Home");
            user.Status = false;
            await UserManager.UpdateAsync(user);
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ForgotPassword() => View();

        [HttpGet, AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() => View();

        //Forgot user password
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "Обязательное поле для ввода");
                ViewBag.email = email;
                return View();
            }
            string pattern = "[A-Za-z0-9._%+-]+(@mail.ru|@gmail.com)$";
            if (!Regex.IsMatch(email, pattern))
            {
                ModelState.AddModelError("email", "Некорректный адрес (@mail.ru или @gmail.com)");
                ViewBag.email = email;
                return View();
            }
            var user = UserManager.FindByEmail(email);
            if (user == null)
                return RedirectToAction("Error", "Home");
            string message = string.Format("<p>Для сброса пароля перейдите по ссылке:" +
                "<a href=\"{0}\" title=\"Подтвердить почту\">{0}</a></p>",
                Url.Action("ResetPassword", "Account", new { userId = user.Id, user.Email }, Request.Url.Scheme));
            EmailServices.Email.ComfirmEmailMessage(user.Email, message, "Сброс пароля");
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string userId, string email)
        {
            user = await UserManager.FindByIdAsync(userId);
            if (user == null || email == null)
                return RedirectToAction("Error", "Home");
            var code = await UserManager.GeneratePasswordResetTokenAsync(userId);
            ResetPasswordViewModel resetPasswordView = new() { Id = userId, Email = email, Code = code };
            return View(resetPasswordView);
        }

        //Reset password
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ResetPasswordAsync(model.Id, model.Code, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Authorize(Roles = "programmer")]
        public async  Task<ActionResult> EditRole(string userId)
        {
            user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Error", "Home");
            ChangeRoleViewModel model = new()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = await UserManager.GetRolesAsync(userId),
                AllRoles = RoleManager.Roles.ToList(),
            };
            return PartialView(model);
        }

        //Set roles for user
        [HttpPost, Authorize(Roles = "programmer")]
        public async Task<ActionResult> EditRole(string userId, List<string> roles)
        {
            user = db.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Error", "Home");
            IList<string> userRoles = await UserManager.GetRolesAsync(userId);
            foreach (var addrole in roles.Except(userRoles))
                await UserManager.AddToRoleAsync(userId, addrole);
            foreach (var removerole in userRoles.Except(roles))
                await UserManager.RemoveFromRolesAsync(userId, removerole);
            if (roles.Count == 0)
            {
                await UserManager.AddToRoleAsync(userId, "user");
                return await EditRole(userId);
            }
            User programmerUser = await UserManager.FindByNameAsync(User.Identity.Name);
            ClaimsIdentity claim = await UserManager.CreateIdentityAsync(programmerUser, 
                DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
            return RedirectToAction("ListUser", "Home");
        }
    }
}
