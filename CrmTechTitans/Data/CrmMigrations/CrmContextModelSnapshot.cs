﻿// <auto-generated />
using System;
using CrmTechTitans.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CrmTechTitans.Data.CrmMigrations
{
    [DbContext(typeof(CrmContext))]
    partial class CrmContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("CrmTechTitans.Models.Address", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<int>("Province")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Contact", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Linkedin")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Industry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("NAICS")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Industries");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Interaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Person")
                        .HasColumnType("TEXT");

                    b.Property<string>("interaction")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Interactions");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.InteractionMember", b =>
                {
                    b.Property<int>("InteractionMemberID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("InteractionID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.HasKey("InteractionMemberID");

                    b.HasIndex("InteractionID");

                    b.HasIndex("MemberID");

                    b.ToTable("InteractionMembers");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberAddress", b =>
                {
                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AddressID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AddressType")
                        .HasColumnType("INTEGER");

                    b.HasKey("MemberID", "AddressID");

                    b.HasIndex("AddressID");

                    b.ToTable("MemberAddresses");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberContact", b =>
                {
                    b.Property<int>("ContactID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactType")
                        .HasColumnType("INTEGER");

                    b.HasKey("ContactID", "MemberID");

                    b.HasIndex("MemberID");

                    b.ToTable("MemberContacts");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberIndustry", b =>
                {
                    b.Property<int>("IndustryID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.HasKey("IndustryID", "MemberID");

                    b.HasIndex("MemberID");

                    b.ToTable("IndustryMembers");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberOpportunity", b =>
                {
                    b.Property<int>("OpportunityID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberID")
                        .HasColumnType("INTEGER");

                    b.HasKey("OpportunityID", "MemberID");

                    b.HasIndex("MemberID");

                    b.ToTable("MemberOpportunities");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Member", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CompanySize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyWebsite")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactedBy")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastContactDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("MemberSince")
                        .HasColumnType("TEXT");

                    b.Property<int>("MembershipStatus")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MembershipType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Opportunity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Opportunities");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.InteractionMember", b =>
                {
                    b.HasOne("CrmTechTitans.Models.Interaction", "Interaction")
                        .WithMany("InteractionMembers")
                        .HasForeignKey("InteractionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CrmTechTitans.Models.Member", "Member")
                        .WithMany("InteractionMembers")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Interaction");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberAddress", b =>
                {
                    b.HasOne("CrmTechTitans.Models.Address", "Address")
                        .WithMany("MemberAddresses")
                        .HasForeignKey("AddressID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CrmTechTitans.Models.Member", "Member")
                        .WithMany("MemberAddresses")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberContact", b =>
                {
                    b.HasOne("CrmTechTitans.Models.Contact", "Contact")
                        .WithMany("MemberContacts")
                        .HasForeignKey("ContactID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CrmTechTitans.Models.Member", "Member")
                        .WithMany("MemberContacts")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberIndustry", b =>
                {
                    b.HasOne("CrmTechTitans.Models.Industry", "Industry")
                        .WithMany("IndustryMembers")
                        .HasForeignKey("IndustryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CrmTechTitans.Models.Member", "Member")
                        .WithMany("IndustryMembers")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Industry");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CrmTechTitans.Models.JoinTables.MemberOpportunity", b =>
                {
                    b.HasOne("CrmTechTitans.Models.Member", "Member")
                        .WithMany("MemberOpportunities")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CrmTechTitans.Models.Opportunity", "Opportunity")
                        .WithMany("MemberOpportunities")
                        .HasForeignKey("OpportunityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Opportunity");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Address", b =>
                {
                    b.Navigation("MemberAddresses");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Contact", b =>
                {
                    b.Navigation("MemberContacts");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Industry", b =>
                {
                    b.Navigation("IndustryMembers");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Interaction", b =>
                {
                    b.Navigation("InteractionMembers");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Member", b =>
                {
                    b.Navigation("IndustryMembers");

                    b.Navigation("InteractionMembers");

                    b.Navigation("MemberAddresses");

                    b.Navigation("MemberContacts");

                    b.Navigation("MemberOpportunities");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Opportunity", b =>
                {
                    b.Navigation("MemberOpportunities");
                });
#pragma warning restore 612, 618
        }
    }
}
