using Microsoft.EntityFrameworkCore;
using Sir98Backend.Models;

namespace Sir98Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivitySubscription> ActivitySubscriptions { get; set; }
        public DbSet<ChangedActivity> ChangedActivities { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<PushSubscription> PushSubscriptions { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAwaitActivation> UsersAwaitingActivation { get; set; }



        // Not putting VapidConfig in use for now. Unsure if it is meant to go in db at all.
        //public DbSet<VapidConfig> VapidConfigs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------- Activity --------
            modelBuilder.Entity<Activity>(builder =>
            {
                builder.ToTable("Activities");

                builder.HasKey(a => a.Id);

                builder.Property(a => a.Title)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(a => a.Address)
                       .IsRequired()
                       .HasMaxLength(300);

                builder.Property(a => a.Image)
                       .HasMaxLength(500);

                builder.Property(a => a.Link)
                       .HasMaxLength(500);

                builder.Property(a => a.Tag)
                       .HasMaxLength(100);

                builder.Property(a => a.Description)
                       .HasMaxLength(250);

                builder.Property(a => a.StartUtc)
                       .IsRequired();

                builder.Property(a => a.EndUtc)
                       .IsRequired();

                builder.Property(a => a.IsRecurring)
                       .IsRequired();

                builder.Property(a => a.Cancelled)
                       .IsRequired();

                //Activity <-> Instructor
                builder
                    .HasMany(a => a.Instructors)
                    .WithMany(i => i.Activities)
                    .UsingEntity(j =>
                        j.ToTable("ActivityInstructors")); // join table name

                //Activity 1 -> * ActivitySubscription
                builder
                    .HasMany(a => a.ActivitySubscriptions)
                    .WithOne(s => s.Activity)
                    .HasForeignKey(s => s.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 0 or 1 ChangedActivity
                // Activity 1 -> * ChangedActivities
                builder
                    .HasMany(a => a.ChangedActivities)
                    .WithOne(ca => ca.Activity)
                    .HasForeignKey(ca => ca.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // -------- ActivitySubscription --------
            modelBuilder.Entity<ActivitySubscription>(builder =>
            {
                builder.ToTable("ActivitySubscriptions");

                builder.HasKey(s => s.Id);

                builder.Property(s => s.UserId)
                       .IsRequired();

                builder.Property(s => s.OriginalStartUtc)
                       .IsRequired();

                // each subscription belongs to exactly one user
                builder
                    .HasOne(s => s.User)
                    .WithMany(u => u.ActivitySubscriptions)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);


                // prevent duplicate subscription for same occurrence
                builder
                    .HasIndex(s => new { s.UserId, s.ActivityId, s.OriginalStartUtc })
                    .IsUnique();
                // Speeds up "who is subscribed to this activity" lookups (NotificationService series-change case)
                builder.HasIndex(s => s.ActivityId);
                // Speeds up occurrence targeting (ActivityId + OriginalStartUtc) and helps filtering
                builder.HasIndex(s => new { s.ActivityId, s.OriginalStartUtc, s.AllOccurrences });

            });

            // -------- Instructor --------
            modelBuilder.Entity<Instructor>(builder =>
            {
                builder.ToTable("Instructors");

                builder.HasKey(i => i.Id);

                builder.Property(i => i.Email)
                       .HasMaxLength(100);

                builder.Property(i => i.Number)
                       .HasMaxLength(100);

                builder.Property(i => i.FirstName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(i => i.Image)
                       .HasMaxLength(500);

                // many-to-many with activities configured in Activity entity
            });

            // -------- User --------
            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("Users");

                builder.HasKey(u => u.Email); // Using Email as PK

                builder.Property(u => u.Email)
                       .IsRequired()
                       .HasMaxLength(256);

                builder.Property(u => u.HashedPassword)
                       .IsRequired();

                builder.Property(u => u.Role)
                       .IsRequired()
                       .HasMaxLength(50);
            });

            // -------- ChangedActivity --------
            modelBuilder.Entity<ChangedActivity>(builder =>
            {
                builder.ToTable("ChangedActivities");

                builder.HasKey(ca => ca.Id);

                // Ensure one change per (ActivityId, OriginalStartUtc)
                builder.HasIndex(ca => new { ca.ActivityId, ca.OriginalStartUtc }).IsUnique();


                builder.Property(ca => ca.ActivityId)
                       .IsRequired();

                //All these nullable props should mirror MaxLength of Activity props

                builder.Property(ca => ca.OriginalStartUtc)
                       .IsRequired();

                builder.Property(ca => ca.IsCancelled)
                       .IsRequired();

                builder.Property(ca => ca.NewTitle)
                       .HasMaxLength(200);

                builder.Property(ca => ca.NewDescription)
                       .HasMaxLength(250);

                builder.Property(ca => ca.NewAddress)
                       .HasMaxLength(300);

                builder.Property(ca => ca.NewTag)
                       .HasMaxLength(100);

                // For each ChangedActivity: exactly 1 Activity
                builder
              .HasOne(ca => ca.Activity)
              .WithMany(a => a.ChangedActivities)
              .HasForeignKey(ca => ca.ActivityId)
              .OnDelete(DeleteBehavior.Cascade);
            });

            // -------- UserAwaitActivation --------
            modelBuilder.Entity<UserAwaitActivation>(builder =>
            {
                builder.ToTable("UsersAwaitingActivation");

                // Pick a key. ActivationCode is a good natural PK here.
                builder.HasKey(x => x.ActivationCode);

                builder.Property(x => x.ActivationCode)
                       .IsRequired()
                       .HasMaxLength(128);

                builder.Property(x => x.Email)
                       .IsRequired()
                       .HasMaxLength(256);

                builder.Property(x => x.HashedPassword)
                       .IsRequired();

                builder.Property(x => x.ExpirationDate)
                       .IsRequired();

                // Prevent duplicates (matches your old in-memory checks)
                builder.HasIndex(x => x.Email).IsUnique();
            });
            // -------- PushSubscription --------
            modelBuilder.Entity<PushSubscription>(builder =>
            {
                builder.ToTable("PushSubscriptions");

                builder.HasKey(ps => ps.Id);

                builder.Property(ps => ps.UserId)
                    .IsRequired();

                builder.Property(ps => ps.Endpoint)
                    .IsRequired();

                builder.Property(ps => ps.P256dh)
                    .IsRequired();

                builder.Property(ps => ps.Auth)
                    .IsRequired();

                builder.Property(ps => ps.CreatedAtUtc)
                    .IsRequired();

                builder.Property(ps => ps.LastUsedUtc)
                    .IsRequired();

                // One endpoint must only exist once in the system
                builder.HasIndex(ps => ps.Endpoint)
                    .IsUnique();

                // Fast index for querying subscriptions by user
                builder.HasIndex(ps => ps.UserId);
            });

        }
    }
}
