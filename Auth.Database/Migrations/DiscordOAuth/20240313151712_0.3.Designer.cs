﻿// <auto-generated />
using System;
using Auth.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Auth.Database.Migrations.DiscordOAuth
{
    [DbContext(typeof(DiscordOAuthContext))]
    [Migration("20240313151712_0.3")]
    partial class _03
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Auth.Database.Model.DiscordOAuthModel", b =>
                {
                    b.Property<string>("State")
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("Accent_color")
                        .HasColumnType("int");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<string>("Avatar_decoration")
                        .HasColumnType("longtext");

                    b.Property<string>("Banner")
                        .HasColumnType("longtext");

                    b.Property<bool?>("Bot")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Discriminator")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Expires_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Flags")
                        .HasColumnType("int");

                    b.Property<string>("Global_name")
                        .HasColumnType("longtext");

                    b.Property<ulong>("Id")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Locale")
                        .HasColumnType("longtext");

                    b.Property<bool?>("Mfa_enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("Premium_type")
                        .HasColumnType("int");

                    b.Property<int?>("Public_flags")
                        .HasColumnType("int");

                    b.Property<bool?>("System")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Username")
                        .HasColumnType("longtext");

                    b.Property<bool?>("Verified")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("State");

                    b.ToTable("OAuth");
                });
#pragma warning restore 612, 618
        }
    }
}
