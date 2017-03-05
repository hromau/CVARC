using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CvarcWeb.Data;

namespace CvarcWeb.Migrations
{
    [DbContext(typeof(CvarcDbContext))]
    [Migration("20170218073716_stringToGuid")]
    partial class stringToGuid
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CvarcWeb.Models.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GameName");

                    b.Property<string>("PathToLog");

                    b.HasKey("GameId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("CvarcWeb.Models.Result", b =>
                {
                    b.Property<int>("ResultId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Scores");

                    b.Property<string>("ScoresType");

                    b.Property<int?>("TeamGameResultId");

                    b.HasKey("ResultId");

                    b.HasIndex("TeamGameResultId");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("CvarcWeb.Models.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CvarcTag");

                    b.Property<string>("LinkToImage");

                    b.Property<string>("Name");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("CvarcWeb.Models.TeamGameResult", b =>
                {
                    b.Property<int>("TeamGameResultId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("GameId");

                    b.Property<int?>("TeamId");

                    b.HasKey("TeamGameResultId");

                    b.HasIndex("GameId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamGameResults");
                });

            modelBuilder.Entity("CvarcWeb.Models.Result", b =>
                {
                    b.HasOne("CvarcWeb.Models.TeamGameResult", "TeamGameResult")
                        .WithMany("Results")
                        .HasForeignKey("TeamGameResultId");
                });

            modelBuilder.Entity("CvarcWeb.Models.TeamGameResult", b =>
                {
                    b.HasOne("CvarcWeb.Models.Game", "Game")
                        .WithMany("TeamGameResults")
                        .HasForeignKey("GameId");

                    b.HasOne("CvarcWeb.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });
        }
    }
}
