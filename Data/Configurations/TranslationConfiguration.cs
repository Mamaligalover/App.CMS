using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.CMS.Data.Entities;

namespace App.CMS.Data.Configurations;

public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable("Translations");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.TranslatedText)
            .HasMaxLength(500);

        builder.Property(t => t.Pronunciation)
            .HasMaxLength(200);

        builder.Property(t => t.Definition)
            .HasMaxLength(1500);

        builder.Property(t => t.UsageExample)
            .HasMaxLength(1000);

        builder.Property(t => t.Notes)
            .HasMaxLength(1000);

        builder.Property(t => t.Language)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(t => t.Word)
            .WithMany(w => w.Translations)
            .HasForeignKey(t => t.WordId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.WordId);
        builder.HasIndex(t => t.Language);
        builder.HasIndex(t => new { t.WordId, t.Language }).IsUnique();
    }
}