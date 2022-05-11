using System;
using System.Collections.Generic;
using CollegeBackend.Objects.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollegeBackend
{
    public partial class CollegeBackendContext : DbContext
    {
        public CollegeBackendContext()
        {
        }

        public CollegeBackendContext(DbContextOptions<CollegeBackendContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Active> Actives { get; set; } = null!;
        public virtual DbSet<Carriage> Carriages { get; set; } = null!;
        public virtual DbSet<Direction> Directions { get; set; } = null!;
        public virtual DbSet<Sitting> Sittings { get; set; } = null!;
        public virtual DbSet<Station> Stations { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<Train> Trains { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) throw new ApplicationException("Looks like connection is not configured properly!");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Active>(entity =>
            {
                entity.ToTable("active");

                entity.Property(e => e.ActiveId).HasColumnName("active_id");

                entity.Property(e => e.MainDirectionId).HasColumnName("main_direction_id");

                entity.Property(e => e.MainStartDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("main_start_date_time");

                entity.Property(e => e.StartDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date_time");

                entity.Property(e => e.StationId).HasColumnName("station_id");

                entity.Property(e => e.TrainId).HasColumnName("train_id");

                entity.HasOne(d => d.MainDirection)
                    .WithMany(p => p.Actives)
                    .HasForeignKey(d => d.MainDirectionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_active_directions_1");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.Actives)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_active_stations_1");

                entity.HasOne(d => d.Train)
                    .WithOne(p => p.Active)
                    .HasForeignKey<Active>(d => d.TrainId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_active_trains_1");
            });

            modelBuilder.Entity<Carriage>(entity =>
            {
                entity.ToTable("carriages");

                entity.Property(e => e.CarriageId).HasColumnName("carriage_id");

                entity.Property(e => e.Index)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("index")
                    .IsFixedLength();

                entity.Property(e => e.RelatedTrainId).HasColumnName("related_train_id");

                entity.HasOne(d => d.RelatedTrain)
                    .WithMany(p => p.Carriages)
                    .HasForeignKey(d => d.RelatedTrainId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_carriages_trains_1");
            });

            modelBuilder.Entity<Direction>(entity =>
            {
                entity.ToTable("directions");

                entity.Property(e => e.DirectionId).HasColumnName("direction_id");

                entity.Property(e => e.EndStationId).HasColumnName("end_station_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.StartStationId).HasColumnName("start_station_id");
            });

            modelBuilder.Entity<Sitting>(entity =>
            {
                entity.HasKey(e => e.SitId)
                    .HasName("_copy_6");

                entity.ToTable("sitting");

                entity.Property(e => e.SitId).HasColumnName("sit_id");

                entity.Property(e => e.Index)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("index")
                    .IsFixedLength();

                entity.Property(e => e.RelatedCarriageId).HasColumnName("related_carriage_id");

                entity.HasOne(d => d.RelatedCarriage)
                    .WithMany(p => p.Sittings)
                    .HasForeignKey(d => d.RelatedCarriageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_sitting_carriages_1");
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.ToTable("stations");

                entity.Property(e => e.StationId).HasColumnName("station_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.RelatedDirection).HasColumnName("related_direction");

                entity.HasOne(d => d.RelatedDirectionNavigation)
                    .WithMany(p => p.Stations)
                    .HasForeignKey(d => d.RelatedDirection)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_stations_directions_1");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("tickets");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.Property(e => e.EndStationId).HasColumnName("end_station_id");

                entity.Property(e => e.PassportId).HasColumnName("passport_id");

                entity.Property(e => e.RelatedActiveId).HasColumnName("related_active_id");

                entity.Property(e => e.RelatedDirectionId).HasColumnName("related_direction_id");

                entity.Property(e => e.SittingId).HasColumnName("sitting_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.EndStation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.EndStationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_tickets_stations_1");

                entity.HasOne(d => d.Passport)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.PassportId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_users_tickets_1");

                entity.HasOne(d => d.RelatedActive)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.RelatedActiveId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_tickets_active_1");

                entity.HasOne(d => d.RelatedDirection)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.RelatedDirectionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_tickets_directions_1");

                entity.HasOne(d => d.Sitting)
                    .WithOne(p => p.Ticket)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_sitting_tickets_1");
            });

            modelBuilder.Entity<Train>(entity =>
            {
                entity.ToTable("trains");

                entity.Property(e => e.TrainId).HasColumnName("train_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.PassportId)
                    .HasName("PK__users__EC9E90ED125B0E19");

                entity.ToTable("users");

                entity.Property(e => e.PassportId)
                    .ValueGeneratedNever()
                    .HasColumnName("passport_id");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(255)
                    .HasColumnName("patronymic");

                entity.Property(e => e.Role)
                    .HasMaxLength(255)
                    .HasColumnName("role");

                entity.Property(e => e.SecondName)
                    .HasMaxLength(255)
                    .HasColumnName("second_name");

                entity.Property(e => e.Username).HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}