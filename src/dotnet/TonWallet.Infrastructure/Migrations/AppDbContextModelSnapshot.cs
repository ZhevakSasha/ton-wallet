﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TonWallet.Infrastructure;

#nullable disable

namespace TonWallet.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TonWallet.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WalletAddressId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WalletAddressId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TonWallet.Domain.Entities.WalletAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("RawForm")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WalletAddresses");
                });

            modelBuilder.Entity("TonWallet.Domain.Entities.User", b =>
                {
                    b.HasOne("TonWallet.Domain.Entities.WalletAddress", "WalletAddresses")
                        .WithMany()
                        .HasForeignKey("WalletAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WalletAddresses");
                });

            modelBuilder.Entity("TonWallet.Domain.Entities.WalletAddress", b =>
                {
                    b.OwnsOne("TonWallet.Domain.Entities.AddressInfo", "Bounceable", b1 =>
                        {
                            b1.Property<int>("WalletAddressId")
                                .HasColumnType("integer");

                            b1.Property<string>("B64")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("B64url")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("WalletAddressId");

                            b1.ToTable("WalletAddresses");

                            b1.WithOwner()
                                .HasForeignKey("WalletAddressId");
                        });

                    b.OwnsOne("TonWallet.Domain.Entities.AddressInfo", "NonBounceable", b1 =>
                        {
                            b1.Property<int>("WalletAddressId")
                                .HasColumnType("integer");

                            b1.Property<string>("B64")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("B64url")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("WalletAddressId");

                            b1.ToTable("WalletAddresses");

                            b1.WithOwner()
                                .HasForeignKey("WalletAddressId");
                        });

                    b.Navigation("Bounceable")
                        .IsRequired();

                    b.Navigation("NonBounceable")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
