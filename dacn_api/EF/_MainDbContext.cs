using dacn_api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace dacn_api.EF;

public partial class _MainDbContext : IdentityDbContext<Account>
{
    public _MainDbContext()
    {
    }

    public _MainDbContext(DbContextOptions<_MainDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityRecord> ActivityRecords { get; set; }

    public virtual DbSet<DailyGoal> DailyGoals { get; set; }

    public virtual DbSet<ExerciseLibrary> ExerciseLibraries { get; set; }

    public virtual DbSet<FoodDatabase> FoodDatabases { get; set; }

    public virtual DbSet<HealthMetric> HealthMetrics { get; set; }

    public virtual DbSet<MealItem> MealItems { get; set; }

    public virtual DbSet<MealRecord> MealRecords { get; set; }

    public virtual DbSet<MedicationReminder> MedicationReminders { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<SleepRecord> SleepRecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGoal> UserGoals { get; set; }

    public virtual DbSet<WaterIntakeRecord> WaterIntakeRecords { get; set; }

    public virtual DbSet<WeightRecord> WeightRecords { get; set; }

    public virtual DbSet<WorkoutPlan> WorkoutPlans { get; set; }

    public virtual DbSet<WorkoutPlanExercise> WorkoutPlanExercises { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //
        modelBuilder.Entity<ActivityRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activity__3214EC075CFD5F85");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActivityR__UserI__5BE2A6F2");
        });

        modelBuilder.Entity<DailyGoal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DailyGoa__3214EC0772E47903");

            entity.Property(e => e.CurrentValue).HasDefaultValue(0.0);
            entity.Property(e => e.IsCompleted).HasComputedColumnSql("(case when [CurrentValue]>=[TargetValue] then (1) else (0) end)", true);
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.DailyGoals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyGoal__UserI__10566F31");
        });

        modelBuilder.Entity<ExerciseLibrary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exercise__3214EC07293A675C");
        });

        modelBuilder.Entity<FoodDatabase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FoodData__3214EC077252CDDF");
        });

        modelBuilder.Entity<HealthMetric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HealthMe__3214EC07DFA88854");

            entity.HasOne(d => d.User).WithMany(p => p.HealthMetrics)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HealthMet__UserI__5EBF139D");
        });

        modelBuilder.Entity<MealItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MealItem__3214EC0799F99B78");

            entity.HasOne(d => d.Food).WithMany(p => p.MealItems).HasConstraintName("FK__MealItems__FoodI__5629CD9C");

            entity.HasOne(d => d.MealRecord).WithMany(p => p.MealItems).HasConstraintName("FK__MealItems__MealR__5535A963");
        });

        modelBuilder.Entity<MealRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MealReco__3214EC078F3BF0DE");

            entity.HasOne(d => d.User).WithMany(p => p.MealRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MealRecor__UserI__52593CB8");
        });

        modelBuilder.Entity<MedicationReminder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicati__3214EC076BE719C3");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.MedicationReminders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medicatio__UserI__03F0984C");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07B15AA5A3");

            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__656C112C");
        });

        modelBuilder.Entity<SleepRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SleepRec__3214EC0713CBF08D");

            entity.Property(e => e.DurationMinutes).HasComputedColumnSql("(datediff(minute,[StartTime],[EndTime]))", true);

            entity.HasOne(d => d.User).WithMany(p => p.SleepRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SleepReco__UserI__06CD04F7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079E05D9FF");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserGoal__3214EC071B500F13");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StartDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.UserGoals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserGoals__UserI__0B91BA14");
        });

        modelBuilder.Entity<WaterIntakeRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WaterInt__3214EC0773B3921A");

            entity.HasOne(d => d.User).WithMany(p => p.WaterIntakeRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WaterInta__UserI__59063A47");
        });

        modelBuilder.Entity<WeightRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WeightRe__3214EC070C8A55B2");

            entity.HasOne(d => d.User).WithMany(p => p.WeightRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WeightRec__UserI__4D94879B");
        });

        modelBuilder.Entity<WorkoutPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkoutP__3214EC073CEE4AC2");

            entity.HasOne(d => d.User).WithMany(p => p.WorkoutPlans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkoutPl__UserI__619B8048");
        });

        modelBuilder.Entity<WorkoutPlanExercise>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkoutP__3214EC075701A401");

            entity.HasOne(d => d.Exercise).WithMany(p => p.WorkoutPlanExercises)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkoutPl__Exerc__18EBB532");

            entity.HasOne(d => d.WorkoutPlan).WithMany(p => p.WorkoutPlanExercises).HasConstraintName("FK__WorkoutPl__Worko__17F790F9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
