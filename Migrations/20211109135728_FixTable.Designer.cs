// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebMathModelLabs.Data;

namespace WebMathModelLabs.Migrations
{
    [DbContext(typeof(LabDataContext))]
    [Migration("20211109135728_FixTable")]
    partial class FixTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("WebMathModelLabs.Entity.MathLabTable1", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("SValue")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("MathLabTable1");
                });
#pragma warning restore 612, 618
        }
    }
}
