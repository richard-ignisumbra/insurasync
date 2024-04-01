using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[PrimaryKey("ApplicationId", "SectionId")]
[Table("ApplicationSectionResponse")]
public partial class ApplicationSectionResponse
{
    [Key]
    [Column("applicationId")]
    public int ApplicationId { get; set; }

    [Key]
    [Column("sectionId")]
    public int SectionId { get; set; }

    [Column("isCompleted")]
    public bool IsCompleted { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("ApplicationSectionResponses")]
    public virtual Application Application { get; set; }

    [ForeignKey("SectionId")]
    [InverseProperty("ApplicationSectionResponses")]
    public virtual ApplicationSection Section { get; set; }
}
