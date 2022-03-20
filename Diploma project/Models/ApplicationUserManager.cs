using Diploma_project.App_Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Diploma_project.Models
{
    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store)
                : base(store)
        {
            var provider = new DpapiDataProtectionProvider("YourAppName");
            UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("SampleTokenName"));
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(
              new UserStore<User>(context.Get<PortalContext>()));
            return manager;
        }
    }
}
