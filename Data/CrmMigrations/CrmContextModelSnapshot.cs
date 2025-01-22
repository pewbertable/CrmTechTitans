﻿// <auto-generated />
using System;
using CRM.Data;
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

            modelBuilder.Entity("CRM.Models.Address", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("Province")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Zip")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("CRM.Models.Contact", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("CRM.Models.Opportunity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Priority")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Opportunities");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Interaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("interaction")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("interactions");
                });

            modelBuilder.Entity("CrmTechTitans.Models.Member", b =>
                {
                    b.Property<int>("Id")
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

                    b.Property<int>("Industry")
                        .HasColumnType("INTEGER");

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

                    b.HasKey("Id");

                    b.ToTable("Members");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CompanySize = 3,
                            CompanyWebsite = "https://www.techsolutions.com",
                            ContactedBy = "John Doe",
                            Industry = 334,
                            LastContactDate = new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Tech Solutions Inc.",
                            MemberSince = new DateTime(2018, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 0,
                            Notes = "Key partner for software development."
                        },
                        new
                        {
                            Id = 2,
                            CompanySize = 4,
                            CompanyWebsite = "https://www.energycorp.com",
                            ContactedBy = "Jane Smith",
                            Industry = 211,
                            LastContactDate = new DateTime(2023, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Energy Corp",
                            MemberSince = new DateTime(2015, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 3,
                            Notes = "Major supplier of renewable energy."
                        },
                        new
                        {
                            Id = 3,
                            CompanySize = 2,
                            CompanyWebsite = "https://www.greeninnovations.com",
                            ContactedBy = "Albert Green",
                            Industry = 325,
                            LastContactDate = new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Green Innovations Ltd.",
                            MemberSince = new DateTime(2017, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 1,
                            Notes = "Innovators in green technology solutions."
                        },
                        new
                        {
                            Id = 4,
                            CompanySize = 1,
                            CompanyWebsite = "https://www.citylogistics.com",
                            ContactedBy = "Maria Johnson",
                            Industry = 484,
                            LastContactDate = new DateTime(2023, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "City Logistics",
                            MemberSince = new DateTime(2020, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 1,
                            MembershipType = 3,
                            Notes = "Local courier and delivery services."
                        },
                        new
                        {
                            Id = 5,
                            CompanySize = 4,
                            CompanyWebsite = "https://www.healthcareplus.com",
                            ContactedBy = "Emily Wright",
                            Industry = 441,
                            LastContactDate = new DateTime(2023, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "HealthCare Plus",
                            MemberSince = new DateTime(2012, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 2,
                            Notes = "Leading healthcare services provider."
                        },
                        new
                        {
                            Id = 6,
                            CompanySize = 3,
                            CompanyWebsite = "https://www.techelectronics.com",
                            ContactedBy = "James Carter",
                            Industry = 335,
                            LastContactDate = new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Tech Electronics",
                            MemberSince = new DateTime(2016, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 4,
                            Notes = "Global leader in electronic components."
                        },
                        new
                        {
                            Id = 7,
                            CompanySize = 2,
                            CompanyWebsite = "https://www.autopartsco.com",
                            ContactedBy = "Daniel Lee",
                            Industry = 441,
                            LastContactDate = new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Auto Parts Co.",
                            MemberSince = new DateTime(2019, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 0,
                            Notes = "Supplier of auto parts to regional stores."
                        },
                        new
                        {
                            Id = 8,
                            CompanySize = 1,
                            CompanyWebsite = "https://www.foodiesmarket.com",
                            ContactedBy = "Linda Miller",
                            Industry = 311,
                            LastContactDate = new DateTime(2023, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Foodies Market",
                            MemberSince = new DateTime(2021, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 1,
                            Notes = "Specializing in local organic food products."
                        },
                        new
                        {
                            Id = 9,
                            CompanySize = 4,
                            CompanyWebsite = "https://www.globaltransport.com",
                            ContactedBy = "Nina Roberts",
                            Industry = 483,
                            LastContactDate = new DateTime(2023, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Global Transport Ltd.",
                            MemberSince = new DateTime(2014, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 3,
                            Notes = "Leading provider of international shipping solutions."
                        },
                        new
                        {
                            Id = 10,
                            CompanySize = 3,
                            CompanyWebsite = "https://www.universaltechhub.com",
                            ContactedBy = "Oliver Thomas",
                            Industry = 518,
                            LastContactDate = new DateTime(2023, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MemberName = "Universal Tech Hub",
                            MemberSince = new DateTime(2013, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            MembershipStatus = 0,
                            MembershipType = 0,
                            Notes = "Cloud services and data hosting provider."
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
