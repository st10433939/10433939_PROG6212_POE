using _10433939_PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;

namespace _10433939_PROG6212_POE.Data
{
    public class _10433939DbContext : DbContext
    {
        public _10433939DbContext(DbContextOptions<_10433939DbContext> options) : base(options)
        {
        }

        public DbSet<Claim> Claim { get; set; }
        public DbSet<ClaimReview> ClaimReview { get; set; }
        public DbSet<UploadedDocument> UploadedDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Claim>(entity =>
            {
                entity.ToTable("Claim");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LecturerName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.HoursWorked).IsRequired();
                entity.Property(e => e.HourlyRate).IsRequired();
                entity.Property(e => e.Balance).IsRequired();

                entity.Property(e => e.AdditionalNotes).HasMaxLength(500);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.SubmittedDate).HasDefaultValueSql("GETDATE()");

            });

            modelBuilder.Entity<ClaimReview>(entity =>
            {
                entity.ToTable("ClaimReview");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Decision)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.ReviewDate).IsRequired();
            });

            modelBuilder.Entity<UploadedDocument>(entity =>
            {
                entity.ToTable("UploadedDocument");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FileName).IsRequired().HasMaxLength(225);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileSize).HasDefaultValue(1);
                entity.Property(e => e.UploadedDate).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}