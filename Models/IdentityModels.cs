using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using AplikasiSOP.Models.Master;
using AplikasiSOP.Models.Transaksi;
using AplikasiSOP.Models.Master.Summary;
using AplikasiSOP.Models.Master.Status;
using AplikasiSOP.Models.Master.Setting_Summary;
using AplikasiSOP.Models.Master.Form_SerahTerima;

namespace AplikasiSOP.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Nama Lengkap")]
        public string Nama { get; set; }

        [Required]
        [Display(Name = "Kelompok")]
        public string Kelompok { get; set; }

        [Required]
        [Display(Name = "NPP")]
        public string NPP { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //Master Data
        public DbSet<Book> Book { get; set; }
        public DbSet<Bab> Bab { get; set; }
        public DbSet<SubBab> SubBab { get; set; }
        public DbSet<SubSubBab> SubSubBab { get; set; }
        public DbSet<Kelompok> Kelompok { get; set; }
        public DbSet<ActivityTimeline> ActivityTimeline { get; set; }
        public DbSet<PPM> PPM { get; set; }
        public DbSet<ListReport> listReport { get; set; }

        //Master Data Status
        public DbSet<ScheduleStatus> ScheduleStatus { get; set; }
        public DbSet<ProgresStatus> ProgresStatus { get; set; }

        //Master Data Summary
        public DbSet<AspekPedoman> AspekPedoman { get; set; }
        public DbSet<HasilReview> HasilReview { get; set; }
        public DbSet<Updating> Updating { get; set; }
        public DbSet<DasarUpdating> DasarUpdating { get; set; }
        public DbSet<AcuanUpdating> AcuanUpdating { get; set; }
        public DbSet<SubBabProsedur> SubBabProsedur { get; set; }
        public DbSet<SubSubBabProsedur> SubSubBabProsedur { get; set; }

        //Form Serah Terima
        public DbSet<FormSerahTerima> FormSerahTerima { get; set; }

        //Data Summary Setting
        public DbSet<DasarUpdatingSetting> DasarUpdatingSetting { get; set; }
        public DbSet<AcuanUpdatingSetting> AcuanUpdatingSetting { get; set; }
        public DbSet<SubBabProsedurSetting> SubBabProsedurSetting { get; set; }
        public DbSet<SubSubBabProsedurSetting> SubSubBabProsedurSetting { get; set; }

        //Data Transaksi
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<HistoryTransaction> HistoryTransaction { get; set; }
        public DbSet<DetailTransaction> DetailTransaction { get; set; }
        public DbSet<SummaryTransaction> SummaryTransaction { get; set; }

        public ApplicationDbContext()
            : base("AplikasiSOP", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityTimeline>().Property(x => x.PersentaseProgress).HasPrecision(25, 5);
            modelBuilder.Entity<ActivityTimeline>().Property(x => x.SumPersentaseProgress).HasPrecision(25, 5);

            base.OnModelCreating(modelBuilder);
        }
    }
}