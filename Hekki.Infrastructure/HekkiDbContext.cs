using Hekki.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hekki.Infrastructure
{
    public class HekkiDbContext : DbContext
    {
        public HekkiDbContext(DbContextOptions<HekkiDbContext> options)
            : base(options)
        {
        }

        public DbSet<RaceEntity> Races { get; set; }
        public DbSet<RaceParticipantEntity> RaceParticipants { get; set; }
        public DbSet<RegulationEntity> Regulations { get; set; }
        public DbSet<HeatEntity> Heats { get; set; }
        public DbSet<HeatResultEntity> HeatResults { get; set; }
        public DbSet<PilotEntity> Pilots { get; set; }
        public DbSet<HeatEntryEntity> HeatEntries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // PilotEntity
            // -------------------------
            modelBuilder.Entity<PilotEntity>(entity =>
            {
                entity.ToTable("pilots");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ProfileUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.PhotoPath)
                    .HasMaxLength(500);
            });

            // -------------------------
            // RegulationEntity
            // -------------------------
            modelBuilder.Entity<RegulationEntity>(entity =>
            {
                entity.ToTable("regulations");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Json)
                    .IsRequired();

                entity.Property(e => e.Version)
                    .IsRequired();

                entity.Property(e => e.CreationDate)
                    .IsRequired();
            });

            // -------------------------
            // RaceEntity
            // -------------------------
            modelBuilder.Entity<RaceEntity>(entity =>
            {
                entity.ToTable("races");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.Property(e => e.RegulationId)
                    .IsRequired();

                entity.HasOne(e => e.Regulation)
                    .WithMany()
                    .HasForeignKey(e => e.RegulationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------------
            // RaceParticipantEntity
            // -------------------------
            modelBuilder.Entity<RaceParticipantEntity>(entity =>
            {
                entity.ToTable("race_participants");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Team)
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasOne(e => e.Race)
                    .WithMany(r => r.Participants)
                    .HasForeignKey(e => e.RaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Pilot)
                    .WithMany()
                    .HasForeignKey(e => e.PilotId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.RaceId, e.PilotId })
                    .IsUnique();
            });

            // -------------------------
            // HeatEntity
            // -------------------------
            modelBuilder.Entity<HeatEntity>(entity =>
            {
                entity.ToTable("heats");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.HeatNumber)
                    .IsRequired();

                entity.Property(e => e.RoleLabel)
                    .HasMaxLength(100);

                entity.Property(e => e.ConfigurationIndex)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired();

                entity.HasOne(e => e.Race)
                    .WithMany(r => r.Heats)
                    .HasForeignKey(e => e.RaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Regulation)
                    .WithMany()
                    .HasForeignKey(e => e.RegulationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------------
            // HeatEntryEntity
            // -------------------------
            modelBuilder.Entity<HeatEntryEntity>(entity =>
            {
                entity.ToTable("heat_entries");

                entity.Property(e => e.GroupNumber)
                    .IsRequired();

                entity.HasKey(e => new { e.HeatId, e.ParticipantId });

                entity.Property(e => e.SeedOrder)
                    .IsRequired();

                entity.HasOne(e => e.Heat)
                    .WithMany(h => h.HeatEntries)
                    .HasForeignKey(e => e.HeatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Participant)
                    .WithMany(rp => rp.HeatEntries)
                    .HasForeignKey(e => e.ParticipantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------------
            // HeatResultEntity
            // -------------------------
            modelBuilder.Entity<HeatResultEntity>(entity =>
            {
                entity.ToTable("heat_results");

                entity.HasKey(e => new { e.HeatId, e.ParticipantId });

                entity.Property(e => e.Status)
                    .IsRequired();

                entity.HasOne(e => e.Heat)
                    .WithMany(h => h.HeatParticipantResults)
                    .HasForeignKey(e => e.HeatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Participant)
                    .WithMany(rp => rp.HeatResults)
                    .HasForeignKey(e => e.ParticipantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
