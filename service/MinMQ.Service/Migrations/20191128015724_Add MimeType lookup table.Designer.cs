﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MinMq.Service.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    [DbContext(typeof(MessageQueueContext))]
    [Migration("20191128015724_Add MimeType lookup table")]
    partial class AddMimeTypelookuptable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("MinMq.Service.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<int>("QueueId")
                        .HasColumnType("integer");

                    b.Property<long>("ReferenceId")
                        .HasColumnType("bigint");

                    b.HasKey("MessageId");

                    b.HasIndex("QueueId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("MinMq.Service.Models.MimeType", b =>
                {
                    b.Property<string>("MimeTypeId")
                        .HasColumnType("text");

                    b.Property<byte>("ByteKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    b.HasKey("MimeTypeId");

                    b.ToTable("MimeType");
                });

            modelBuilder.Entity("MinMq.Service.Models.Queue", b =>
                {
                    b.Property<int>("QueueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("QueueId");

                    b.ToTable("Queues");
                });

            modelBuilder.Entity("MinMq.Service.Models.Message", b =>
                {
                    b.HasOne("MinMq.Service.Models.Queue", "Queue")
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
