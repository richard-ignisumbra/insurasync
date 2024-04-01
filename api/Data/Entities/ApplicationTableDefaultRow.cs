using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[PrimaryKey("ApplicationSectionId", "SortOrder")]
[Table("ApplicationTableDefaultRow")]
public partial class ApplicationTableDefaultRow
{
    [Key]
    public int ApplicationSectionId { get; set; }

    [Key]
    public int SortOrder { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string Label { get; set; }

    [ForeignKey("ApplicationSectionId")]
    [InverseProperty("ApplicationTableDefaultRows")]
    public virtual ApplicationSection ApplicationSection { get; set; }
}
