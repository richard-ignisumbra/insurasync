using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using permaAPI.Data.Entities;

namespace permaAPI.Data.Contexts;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationElementLabel> ApplicationElementLabels { get; set; }

    public virtual DbSet<ApplicationElementResponse> ApplicationElementResponses { get; set; }

    public virtual DbSet<ApplicationElementType> ApplicationElementTypes { get; set; }

    public virtual DbSet<ApplicationReport> ApplicationReports { get; set; }

    public virtual DbSet<ApplicationSection> ApplicationSections { get; set; }

    public virtual DbSet<ApplicationSectionElement> ApplicationSectionElements { get; set; }

    public virtual DbSet<ApplicationSectionResponse> ApplicationSectionResponses { get; set; }

    public virtual DbSet<ApplicationTableDefaultRow> ApplicationTableDefaultRows { get; set; }

    public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<AttachmentCategory> AttachmentCategories { get; set; }

    public virtual DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<ExportCategory> ExportCategories { get; set; }

    public virtual DbSet<Insured> Insureds { get; set; }

    public virtual DbSet<InsuredContact> InsuredContacts { get; set; }

    public virtual DbSet<InsuredNote> InsuredNotes { get; set; }

    public virtual DbSet<LineofCoverage> LineofCoverages { get; set; }

    public virtual DbSet<NoteCategory> NoteCategories { get; set; }

    public virtual DbSet<PermissionType> PermissionTypes { get; set; }

    public virtual DbSet<ReportType> ReportTypes { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationElementLabel>(entity =>
        {
            entity.Property(e => e.ApplicationId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ApplicationElementResponse>(entity =>
        {
            entity.ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("ApplicationElementResponse_History", "dbo");
                        ttb
                            .HasPeriodStart("StartTime")
                            .HasColumnName("StartTime");
                        ttb
                            .HasPeriodEnd("EndTime")
                            .HasColumnName("EndTime");
                    }));

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationElementResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_applicationid");

            entity.HasOne(d => d.Element).WithMany(p => p.ApplicationElementResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_element");
        });

        modelBuilder.Entity<ApplicationReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Applicat__1C9B4E2D8713EF7E");
        });

        modelBuilder.Entity<ApplicationSection>(entity =>
        {
            entity.HasMany(d => d.ApplicationTypes).WithMany(p => p.ApplicationSections)
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicationSectionApplicationType",
                    r => r.HasOne<ApplicationType>().WithMany()
                        .HasForeignKey("ApplicationTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_applicationtypeid"),
                    l => l.HasOne<ApplicationSection>().WithMany()
                        .HasForeignKey("ApplicationSectionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_sectiontypeid"),
                    j =>
                    {
                        j.HasKey("ApplicationSectionId", "ApplicationTypeId");
                    });

            entity.HasMany(d => d.LineofCoverages).WithMany(p => p.ApplicationSections)
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicationSectionLinesofCoverage",
                    r => r.HasOne<LineofCoverage>().WithMany()
                        .HasForeignKey("LineofCoverage")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_lineofcoverage"),
                    l => l.HasOne<ApplicationSection>().WithMany()
                        .HasForeignKey("ApplicationSectionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_appsectionid"),
                    j =>
                    {
                        j.HasKey("ApplicationSectionId", "LineofCoverage");
                        j.ToTable("ApplicationSectionLinesofCoverage");
                    });
        });

        modelBuilder.Entity<ApplicationSectionElement>(entity =>
        {
            entity.Property(e => e.HideFromExport).HasDefaultValueSql("((0))");
            entity.Property(e => e.IndentSpaces).HasDefaultValueSql("((0))");
            entity.Property(e => e.ShowAllLines).HasDefaultValueSql("((1))");
            entity.Property(e => e.SumValues).HasDefaultValueSql("('0')");

            entity.HasMany(d => d.Lines).WithMany(p => p.Elements)
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicationElementLine",
                    r => r.HasOne<LineofCoverage>().WithMany()
                        .HasForeignKey("LineId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_lineid"),
                    l => l.HasOne<ApplicationSectionElement>().WithMany()
                        .HasForeignKey("ElementId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_elementid"),
                    j =>
                    {
                        j.HasKey("ElementId", "LineId");
                        j.ToTable("ApplicationElementLine");
                    });
        });

        modelBuilder.Entity<ApplicationSectionResponse>(entity =>
        {
            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationSectionResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sectionapplicationId");

            entity.HasOne(d => d.Section).WithMany(p => p.ApplicationSectionResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sectionid");
        });

        modelBuilder.Entity<ApplicationTableDefaultRow>(entity =>
        {
            entity.HasKey(e => new { e.ApplicationSectionId, e.SortOrder }).HasName("PK__Applicat__3AF85DEF09702A07");

            entity.HasOne(d => d.ApplicationSection).WithMany(p => p.ApplicationTableDefaultRows)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_section");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Attachme__C417BD81BF53B9DE");
        });

        modelBuilder.Entity<AttachmentCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Attachme__19093A0BEAEAA661");
        });

        modelBuilder.Entity<InsuredContact>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.InsuredContacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_contactId");

            entity.HasOne(d => d.MemberNavigation).WithMany(p => p.InsuredContacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_insured");
        });

        modelBuilder.Entity<InsuredNote>(entity =>
        {
            entity.Property(e => e.NoteId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<LineofCoverage>(entity =>
        {
            entity.HasMany(d => d.Members).WithMany(p => p.LineofCoverages)
                .UsingEntity<Dictionary<string, object>>(
                    "InsuredLineofCoverage",
                    r => r.HasOne<Insured>().WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_2"),
                    l => l.HasOne<LineofCoverage>().WithMany()
                        .HasForeignKey("LineofCoverage")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_1"),
                    j =>
                    {
                        j.HasKey("LineofCoverage", "MemberId");
                        j.ToTable("InsuredLineofCoverage");
                    });
        });

        modelBuilder.Entity<NoteCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__NoteCate__23CAF1D80FD8CA34");
        });




        modelBuilder.Entity<ReportType>(entity =>
        {
            entity.HasKey(e => e.ReportType1).HasName("PK__ReportTy__4140640A9E722E72");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
