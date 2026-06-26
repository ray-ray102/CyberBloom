using Microsoft.EntityFrameworkCore;

namespace CyberBloom
{
    public class CyberBloomContext : DbContext
    {
        private const string ConnectionString =
            "server=localhost;database=cyberbloom;user=root;password=;";

        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                ConnectionString,
                new MySqlServerVersion(new System.Version(8, 0, 0)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(task => task.TaskID);

                entity.Property(task => task.TaskID)
                    .ValueGeneratedOnAdd();

                entity.Property(task => task.Title)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(task => task.Description)
                    .HasMaxLength(255);

                entity.Property(task => task.ReminderDateTime)
                    .HasColumnType("datetime");

                entity.Property(task => task.ReminderMinutes);

                entity.Property(task => task.IsCompleted)
                    .HasColumnType("boolean")
                    .HasDefaultValue(false);
            });
        }
    }
}
