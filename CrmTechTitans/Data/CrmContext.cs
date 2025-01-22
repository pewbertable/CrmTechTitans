using CRM.Models;
using CrmTechTitans.Data;
using CrmTechTitans.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Data
{
    public class CrmContext : DbContext
    {
        public CrmContext(DbContextOptions<CrmContext> options)
            : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Opportunity> Opportunities { get; set; }

        public DbSet<Interaction> interactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CrmInitializer.Seed(modelBuilder);  // Calls the seed method
        }
    }


}
