using Microsoft.EntityFrameworkCore;

namespace CompanyRegistration.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure Company entity
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.CompanyId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CompanyArabicName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CompanyEnglishName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            // Configure VerificationToken entity
            modelBuilder.Entity<VerificationToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OtpCode).IsRequired().HasMaxLength(6);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Company)
                      .WithMany(c => c.VerificationTokens)
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
