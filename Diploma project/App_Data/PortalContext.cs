

using Diploma_project.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Diploma_project.App_Data
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class PortalContext : IdentityDbContext<User>
    {
        //set global max_allowed_packet=1000000000

        public PortalContext()
            : base("DefaultConnection")
        { }

        public static PortalContext Create()
        {
            return new PortalContext();
        }

        public DbSet<FilesGalery> FilesGaleries { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<ContactInformation> Contacts { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<FilesDocuments> FilesDocuments { get; set; }
        public DbSet<FilesDocumentsComment> FilesDocumentsComments { get; set; }
        public DbSet<FilesDocumentsFavorites> filesDocumentsFavorites { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
