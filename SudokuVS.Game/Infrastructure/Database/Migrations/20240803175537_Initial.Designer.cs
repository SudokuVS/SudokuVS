﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SudokuVS.Game.Infrastructure.Database;

#nullable disable

namespace SudokuVS.Game.Infrastructure.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240803175537_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SudokuVS.Game.Models.PlayerStateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Grid")
                        .IsRequired()
                        .HasMaxLength(1053)
                        .HasColumnType("nvarchar(1053)");

                    b.Property<string>("Hints")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("Side")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("PlayerStates");
                });

            modelBuilder.Entity("SudokuVS.Game.Models.SudokuGameEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InitialGrid")
                        .IsRequired()
                        .HasMaxLength(1053)
                        .HasColumnType("nvarchar(1053)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("SolvedGrid")
                        .IsRequired()
                        .HasMaxLength(1053)
                        .HasColumnType("nvarchar(1053)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Winner")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("SudokuVS.Game.Models.Users.UserIdentityEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SudokuVS.Game.Models.PlayerStateEntity", b =>
                {
                    b.HasOne("SudokuVS.Game.Models.SudokuGameEntity", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SudokuVS.Game.Models.Users.UserIdentityEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SudokuVS.Game.Models.SudokuGameEntity", b =>
                {
                    b.OwnsOne("SudokuVS.Game.Models.SudokuGameOptionsEntity", "Options", b1 =>
                        {
                            b1.Property<Guid>("SudokuGameEntityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("MaxHints")
                                .HasColumnType("int");

                            b1.HasKey("SudokuGameEntityId");

                            b1.ToTable("Games");

                            b1.WithOwner()
                                .HasForeignKey("SudokuGameEntityId");
                        });

                    b.Navigation("Options")
                        .IsRequired();
                });

            modelBuilder.Entity("SudokuVS.Game.Models.SudokuGameEntity", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
