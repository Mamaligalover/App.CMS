using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using App.CMS.Data.Entities;

namespace App.CMS.Data.Configurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable("Contents");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.Property(c => c.Body)
            .IsRequired();

        builder.Property(c => c.Summary)
            .HasMaxLength(1000);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(c => c.Author)
            .WithMany(u => u.Contents)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Contents)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.PublishedAt);
        builder.HasIndex(c => c.AuthorId);
        builder.HasIndex(c => c.CategoryId);
    }
}