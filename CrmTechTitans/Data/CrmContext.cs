using CrmTechTitans.Models;
using CrmTechTitans.Models.JoinTables;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
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
        public DbSet<MemberIndustry> IndustryMembers { get; set; }
        public DbSet<MemberAddress> MemberAddresses { get; set; }
        public DbSet<MemberContact> MemberContacts { get; set; }
        public DbSet<MemberOpportunity> MemberOpportunities { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<MemberMembershipType> MemberMembershipTypes { get; set; }
        public DbSet<InteractionMember> InteractionMembers { get; set; }
        public DbSet<ContactPhoto> ContactPhotos { get; set; }
        public DbSet<ContactThumbnail> ContactThumbnails { get; set; }
        public DbSet<MemberPhoto> MemberPhotos { get; set; }
        public DbSet<MemberThumbnail> MemberThumbnails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship
            modelBuilder.Entity<MemberIndustry>()
                .HasKey(im => new { im.IndustryID, im.MemberID });

            modelBuilder.Entity<MemberIndustry>()
                .HasOne(im => im.Industry)
                .WithMany(i => i.IndustryMembers)
                .HasForeignKey(im => im.IndustryID);

            modelBuilder.Entity<MemberIndustry>()
                .HasOne(im => im.Member)
                .WithMany(m => m.IndustryMembers)
                .HasForeignKey(im => im.MemberID);

            // Configure the many-to-many relationship for MemberAddress
            modelBuilder.Entity<MemberAddress>()
                .HasKey(ma => new { ma.MemberID, ma.AddressID });

            modelBuilder.Entity<MemberAddress>()
                .HasOne(ma => ma.Member)
                .WithMany(m => m.MemberAddresses)
                .HasForeignKey(ma => ma.MemberID);

            modelBuilder.Entity<MemberAddress>()
                .HasOne(ma => ma.Address)
                .WithMany(m => m.MemberAddresses)
                .HasForeignKey(ma => ma.AddressID);

            // Configure Many-to-Many relationship for MemberContact
            modelBuilder.Entity<MemberContact>()
                .HasKey(mc => new { mc.ContactID, mc.MemberID });

            modelBuilder.Entity<MemberContact>()
                .HasOne(mc => mc.Contact)
                .WithMany(c => c.MemberContacts)
                .HasForeignKey(mc => mc.ContactID);

            modelBuilder.Entity<MemberContact>()
                .HasOne(mc => mc.Member)
                .WithMany(m => m.MemberContacts)
                .HasForeignKey(mc => mc.MemberID);

            modelBuilder.Entity<MemberContact>()
                .Property(mc => mc.ContactType)
                .HasConversion<int>();

            // Configure Many-to-Many relationship for MemberOpportunity
            modelBuilder.Entity<MemberOpportunity>()
                .HasKey(mc => new { mc.OpportunityID, mc.MemberID });

            modelBuilder.Entity<MemberOpportunity>()
                .HasOne(mc => mc.Opportunity)
                .WithMany(c => c.MemberOpportunities)
                .HasForeignKey(mc => mc.OpportunityID);

            modelBuilder.Entity<MemberOpportunity>()
                .HasOne(mc => mc.Member)
                .WithMany(m => m.MemberOpportunities)
                .HasForeignKey(mc => mc.MemberID);

            // Configure Many-to-Many relationship for InteractionMember
            modelBuilder.Entity<InteractionMember>()
                .HasKey(im => im.InteractionMemberID);

            modelBuilder.Entity<InteractionMember>()
                .HasOne(im => im.Member)
                .WithMany(m => m.InteractionMembers)
                .HasForeignKey(im => im.MemberID);

            modelBuilder.Entity<InteractionMember>()
                .HasOne(im => im.Interaction)
                .WithMany(i => i.InteractionMembers)
                .HasForeignKey(im => im.InteractionID);

            // Configure Many-to-Many relationship for MemberMembershipType
            modelBuilder.Entity<MemberMembershipType>()
                .HasKey(mmt => new { mmt.MemberID, mmt.MembershipTypeID });

            modelBuilder.Entity<MemberMembershipType>()
                .HasOne(mmt => mmt.Member)
                .WithMany(m => m.MemberMembershipTypes)
                .HasForeignKey(mmt => mmt.MemberID);

            // Configure one-to-many relationship between Interaction and Contact
            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Contact)
                .WithMany() // Assuming Contact doesn't have a collection of Interactions
                .HasForeignKey(i => i.ContactId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
