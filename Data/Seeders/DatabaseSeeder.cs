using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;
using App.CMS.Data.Entities;

namespace App.CMS.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        try
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed admin user if not exists
            if (!await context.Users.AnyAsync(u => u.Email == "admin@admin.com"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@admin.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    FirstName = "System",
                    LastName = "Administrator",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                Console.WriteLine("Default admin user created successfully.");
                Console.WriteLine("Email: admin@admin.com");
                Console.WriteLine("Username: admin");
                Console.WriteLine("Password: admin");
            }

            // Seed sample categories if not exists
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Technology",
                        Slug = "technology",
                        Description = "Technology related content",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Business",
                        Slug = "business",
                        Description = "Business related content",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Category
                    {
                        Name = "Health",
                        Slug = "health",
                        Description = "Health and wellness content",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                Console.WriteLine($"Seeded {categories.Count} categories.");
            }


            Console.WriteLine("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            throw;
        }
    }
}