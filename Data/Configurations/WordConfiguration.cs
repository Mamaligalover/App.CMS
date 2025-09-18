using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.CMS.Data.Entities;
using System.Text.Json;

namespace App.CMS.Data.Configurations;

public class WordConfiguration : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        builder.ToTable("Words");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(w => w.Term)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.PartOfSpeech)
            .HasMaxLength(50);

        builder.Property(w => w.Status)
            .HasConversion<string>()
            .HasDefaultValue(Enums.WordStatus.Draft);

        builder.Property(w => w.Difficulty)
            .HasConversion<string>()
            .HasDefaultValue(Enums.WordDifficulty.Medium);

        builder.Property(w => w.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(w => w.Category)
            .WithMany()
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.CreatedByUser)
            .WithMany()
            .HasForeignKey(w => w.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(w => w.Image)
            .WithMany()
            .HasForeignKey(w => w.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(w => w.Audio)
            .WithMany()
            .HasForeignKey(w => w.AudioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(w => w.Video)
            .WithMany()
            .HasForeignKey(w => w.VideoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(w => w.Term);
        builder.HasIndex(w => w.CategoryId);
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.Difficulty);
        builder.HasIndex(w => w.CreatedAt);
        builder.HasIndex(w => new { w.Term, w.CategoryId }).IsUnique();
    }
}