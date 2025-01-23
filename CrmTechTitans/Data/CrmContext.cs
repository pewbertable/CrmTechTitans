﻿using CrmTechTitans.Models;
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

        public DbSet<MemberAddress> MemberAddresses { get; set; }

        public DbSet<MemberContact> MemberContacts { get; set; }

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

            // Configure the many-to-many relationship for MemberAddress
            modelBuilder.Entity<MemberAddress>()
                .HasKey(ma => new { ma.MemberId, ma.AddressId });

            modelBuilder.Entity<MemberAddress>()
                .HasOne(ma => ma.Member)
                .WithMany(m => m.MemberAddresses)
                .HasForeignKey(ma => ma.MemberId);

            modelBuilder.Entity<MemberAddress>()
                .HasOne(ma => ma.Address)
                .WithMany()
                .HasForeignKey(ma => ma.AddressId);

            //Configure Many-to-Many relationship for MemberContact

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

        }


    }


}
