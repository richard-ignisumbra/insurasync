using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("ApplicationSection")]
public partial class ApplicationSection
{
    [Key]
    public int ApplicationSectionId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string SectionTitle { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string Description { get; set; }

    public int? SortOrder { get; set; }

    public bool ShowinNavigation { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string SubTitle { get; set; }

    [InverseProperty("Section")]
    public virtual ICollection<ApplicationSectionResponse> ApplicationSectionResponses { get; } = new List<ApplicationSectionResponse>();

    [InverseProperty("ApplicationSection")]
    public virtual ICollection<ApplicationTableDefaultRow> ApplicationTableDefaultRows { get; } = new List<ApplicationTableDefaultRow>();

    [ForeignKey("ApplicationSectionId")]
    [InverseProperty("ApplicationSections")]
    public virtual ICollection<ApplicationType> ApplicationTypes { get; } = new List<ApplicationType>();

    [ForeignKey("ApplicationSectionId")]
    [InverseProperty("ApplicationSections")]
    public virtual ICollection<LineofCoverage> LineofCoverages { get; } = new List<LineofCoverage>();
}
