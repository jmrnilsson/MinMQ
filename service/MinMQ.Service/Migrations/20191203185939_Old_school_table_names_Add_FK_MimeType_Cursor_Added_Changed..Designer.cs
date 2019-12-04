﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MinMq.Service.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    [DbContext(typeof(MessageQueueContext))]
    [Migration("20191203185939_Old_school_table_names_Add_FK_MimeType_Cursor_Added_Changed.")]
    partial class Old_school_table_names_Add_FK_MimeType_Cursor_Added_Changed
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("MinMq.Service.Models.tMessage", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Changed")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("HashCode")
                        .HasColumnType("text");

                    b.Property<int>("MimeTypeId")
                        .HasColumnType("integer");

                    b.Property<long>("NextReferenceId")
                        .HasColumnType("bigint");

                    b.Property<int>("QueueId")
                        .HasColumnType("integer");

                    b.Property<long>("ReferenceId")
                        .HasColumnType("bigint");

                    b.HasKey("MessageId");

                    b.HasIndex("MimeTypeId");

                    b.HasIndex("QueueId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("MinMq.Service.Models.tMimeType", b =>
                {
                    b.Property<int>("MimeTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp without time zone");

                    b.Property<byte>("ByteKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    b.Property<DateTime>("Changed")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("MimeTypeId");

                    b.ToTable("tMimeType");
                });

            modelBuilder.Entity("MinMq.Service.Models.tQueue", b =>
                {
                    b.Property<int>("QueueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("timestamp without time zone");

                    b.Property<byte>("ByteKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    b.Property<DateTime>("Changed")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("QueueId");

                    b.ToTable("Queues");
                });

            modelBuilder.Entity("MinMq.Service.Models.tMessage", b =>
                {
                    b.HasOne("MinMq.Service.Models.tMimeType", "MimeType")
                        .WithMany("Messages")
                        .HasForeignKey("MimeTypeId")
                        .HasConstraintName("FK_Message_MimeType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MinMq.Service.Models.tQueue", "Queue")
                        .WithMany("Messages")
                        .HasForeignKey("QueueId")
                        .HasConstraintName("FK_Message_Queue")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}