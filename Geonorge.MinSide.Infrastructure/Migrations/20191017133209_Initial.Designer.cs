﻿// <auto-generated />
using System;
using Geonorge.MinSide.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    [DbContext(typeof(OrganizationContext))]
    [Migration("20191017133209_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.Agreement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AgreementDocumentId");

                    b.Property<int?>("Appendix1Id");

                    b.Property<int?>("Appendix2Id");

                    b.Property<int?>("Appendix3Id");

                    b.Property<int?>("DistributionAgreementId");

                    b.Property<string>("OrganizationNumber");

                    b.HasKey("Id");

                    b.HasIndex("AgreementDocumentId");

                    b.HasIndex("Appendix1Id");

                    b.HasIndex("Appendix2Id");

                    b.HasIndex("Appendix3Id");

                    b.HasIndex("DistributionAgreementId");

                    b.ToTable("AgreementDocuments");
                });

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("FileName");

                    b.Property<int?>("MeetingId");

                    b.Property<string>("Name");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.HasIndex("MeetingId");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.Meeting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Conclusion");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.HasKey("Id");

                    b.ToTable("Meetings");
                });

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.ToDo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Done");

                    b.Property<int?>("MeetingId");

                    b.Property<string>("ResponsibleOrganization");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("MeetingId");

                    b.ToTable("ToDo");
                });

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.Agreement", b =>
                {
                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Document", "AgreementDocument")
                        .WithMany()
                        .HasForeignKey("AgreementDocumentId");

                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Document", "Appendix1")
                        .WithMany()
                        .HasForeignKey("Appendix1Id");

                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Document", "Appendix2")
                        .WithMany()
                        .HasForeignKey("Appendix2Id");

                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Document", "Appendix3")
                        .WithMany()
                        .HasForeignKey("Appendix3Id");

                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Document", "DistributionAgreement")
                        .WithMany()
                        .HasForeignKey("DistributionAgreementId");
                });

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.Document", b =>
                {
                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Meeting")
                        .WithMany("Documents")
                        .HasForeignKey("MeetingId");
                });

            modelBuilder.Entity("Geonorge.MinSide.Infrastructure.Context.ToDo", b =>
                {
                    b.HasOne("Geonorge.MinSide.Infrastructure.Context.Meeting")
                        .WithMany("ToDo")
                        .HasForeignKey("MeetingId");
                });
#pragma warning restore 612, 618
        }
    }
}
