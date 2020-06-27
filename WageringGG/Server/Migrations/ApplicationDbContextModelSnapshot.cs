﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WageringGG.Server.Data;

namespace WageringGG.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WageringGG.Shared.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique();

                    b.ToTable("Games");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Fortnite",
                            NormalizedName = "fortnite"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Modern Warfare",
                            NormalizedName = "modern-warfare"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Apex Legends",
                            NormalizedName = "apex-legends"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Valorant",
                            NormalizedName = "valorant"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Fifa 20",
                            NormalizedName = "fifa20"
                        });
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Notification", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProfileId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Date", "ProfileId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Profile", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(12)")
                        .HasMaxLength(12);

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("NormalizedDisplayName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PublicKey")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedDisplayName")
                        .IsUnique()
                        .HasFilter("[NormalizedDisplayName] IS NOT NULL");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<int>("GamesPlayed")
                        .HasColumnType("int");

                    b.Property<int>("ProfileId")
                        .HasColumnType("int");

                    b.Property<string>("ProfileId1")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(9,2)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("ProfileId1");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.StellarAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Asset")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,7)");

                    b.Property<string>("SecretSeed")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StellarAccount");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Tournament", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<decimal>("Entry")
                        .HasColumnType("decimal(18,7)");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Wager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChallengeCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<decimal?>("MaximumWager")
                        .HasColumnType("decimal(18,7)");

                    b.Property<decimal?>("MinimumWager")
                        .HasColumnType("decimal(18,7)");

                    b.Property<int>("PlayerCount")
                        .HasColumnType("int");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Wagers");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerApproval", b =>
                {
                    b.Property<int>("HostId")
                        .HasColumnType("int");

                    b.Property<int>("ChallengeId")
                        .HasColumnType("int");

                    b.Property<bool?>("Approved")
                        .HasColumnType("bit");

                    b.HasKey("HostId", "ChallengeId");

                    b.HasIndex("ChallengeId");

                    b.ToTable("WagerApprovals");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerChallenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,7)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAccepted")
                        .HasColumnType("bit");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<int>("WagerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("WagerId");

                    b.ToTable("WagerChallenges");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerChallengeBid", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Approved")
                        .HasColumnType("bit");

                    b.Property<int>("ChallengeId")
                        .HasColumnType("int");

                    b.Property<byte>("PayablePt")
                        .HasColumnType("tinyint");

                    b.Property<string>("ProfileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte>("ReceivablePt")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("ProfileId");

                    b.ToTable("WagerChallengeBids");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerHostBid", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Approved")
                        .HasColumnType("bit");

                    b.Property<byte>("PayablePt")
                        .HasColumnType("tinyint");

                    b.Property<string>("ProfileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte>("ReceivablePt")
                        .HasColumnType("tinyint");

                    b.Property<int>("WagerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.HasIndex("WagerId");

                    b.ToTable("WagerHostBids");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Notification", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.Profile", "Profile")
                        .WithMany("Notifications")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Rating", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WageringGG.Shared.Models.Profile", "Profile")
                        .WithMany("Ratings")
                        .HasForeignKey("ProfileId1");
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Tournament", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WageringGG.Shared.Models.Wager", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerApproval", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.WagerChallenge", "Challenge")
                        .WithMany("Approvals")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WageringGG.Shared.Models.WagerHostBid", "Host")
                        .WithMany()
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerChallenge", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.StellarAccount", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("WageringGG.Shared.Models.Wager", "Wager")
                        .WithMany("Challenges")
                        .HasForeignKey("WagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerChallengeBid", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.WagerChallenge", "Challenge")
                        .WithMany("Challengers")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WageringGG.Shared.Models.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WageringGG.Shared.Models.WagerHostBid", b =>
                {
                    b.HasOne("WageringGG.Shared.Models.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WageringGG.Shared.Models.Wager", "Wager")
                        .WithMany("Hosts")
                        .HasForeignKey("WagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
