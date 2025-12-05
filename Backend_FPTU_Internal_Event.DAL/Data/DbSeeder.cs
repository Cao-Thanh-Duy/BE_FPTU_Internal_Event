using Backend_FPTU_Internal_Event.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Backend_FPTU_Internal_Event.DAL.Data
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(ApplicationDbContext context, ILogger<DbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Seed Roles
                await SeedRolesAsync();

                // Seed Admin User
                await SeedAdminUserAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        RoleName = "Admin",
                        RoleDescription = "System administrator with full access to all features and settings"
                    },
                    new Role
                    {
                        RoleName = "Student",
                        RoleDescription = "Regular student user who can register for events and view event information"
                    },
                    new Role
                    {
                        RoleName = "Staff",
                        RoleDescription = "Staff member who can manage and organize events"
                    },
                    new Role
                    {
                        RoleName = "Organizer",
                        RoleDescription = "Event organizer with permissions to create and manage events"
                    }
                };

                await _context.Roles.AddRangeAsync(roles);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Roles seeded successfully: Admin, Student, Staff, Organizer");
            }
            else
            {
                _logger.LogInformation("Roles already exist, skipping role seeding.");
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@fptu.edu.vn";

            if (!await _context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");

                if (adminRole == null)
                {
                    _logger.LogError("Admin role not found. Please ensure roles are seeded first.");
                    return;
                }

                var adminUser = new User
                {
                    UserName = "Administrator",
                    Email = adminEmail,
                    HashPassword = HashPassword("admin123"),
                    RoleId = adminRole.RoleId,
                    Role = adminRole
                };

                await _context.Users.AddAsync(adminUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Admin user created successfully - Email: {adminEmail}");
            }
            else
            {
                _logger.LogInformation($"Admin user already exists with email: {adminEmail}");
            }
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
