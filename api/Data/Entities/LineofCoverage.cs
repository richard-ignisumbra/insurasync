using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("LineofCoverage")]
public partial class LineofCoverage
{
    [Key]
    [Column("LineofCoverage")]
    [StringLength(50)]
    [Unicode(false)]
    public string LineofCoverage1 { get; set; }

    [ForeignKey("LineofCoverage")]
    [InverseProperty("LineofCoverages")]
    public virtual ICollection<ApplicationSection> ApplicationSections { get; } = new List<ApplicationSection>();

    [ForeignKey("LineId")]
    [InverseProperty("Lines")]
    public virtual ICollection<ApplicationSectionElement> Elements { get; } = new List<ApplicationSectionElement>();

    [ForeignKey("LineofCoverage")]
    [InverseProperty("LineofCoverages")]
    public virtual ICollection<Insured> Members { get; } = new List<Insured>();
}
