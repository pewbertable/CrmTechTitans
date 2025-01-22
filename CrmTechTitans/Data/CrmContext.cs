using CRM.Models;
using CrmTechTitans.Models;
using CrmTechTitans.Models.JoinTables;
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

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Industry> Industries { get; set; }

        public DbSet<IndustryMember> IndustryMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship
            modelBuilder.Entity<IndustryMember>()
                .HasKey(im => new { im.IndustryID, im.MemberID });

            modelBuilder.Entity<IndustryMember>()
                .HasOne(im => im.Industry)
                .WithMany(i => i.IndustryMembers)
                .HasForeignKey(im => im.IndustryID);

            modelBuilder.Entity<IndustryMember>()
                .HasOne(im => im.Member)
                .WithMany(m => m.IndustryMembers)
                .HasForeignKey(im => im.MemberID);
        }


    }


}
