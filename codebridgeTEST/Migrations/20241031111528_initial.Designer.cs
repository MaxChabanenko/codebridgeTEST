﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using codebridgeTEST.Data;

#nullable disable

namespace codebridgeTEST.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241031111528_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("codebridgeTEST.Models.Dog", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TailLength")
                        .HasColumnType("int");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Name", "TailLength");

                    b.ToTable("Dogs");

                    b.HasData(
                        new
                        {
                            Name = "Neo",
                            TailLength = 22,
                            Color = "red&amber",
                            Weight = 32
                        },
                        new
                        {
                            Name = "Jessy",
                            TailLength = 7,
                            Color = "black&white",
                            Weight = 14
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
