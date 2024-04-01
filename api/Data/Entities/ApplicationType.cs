using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("ApplicationType")]
public partial class ApplicationType
{
    [Key]
    public int ApplicationTypeId { get; set; }

    [Column("ApplicationType")]
    [StringLength(200)]
    [Unicode(false)]
    public string ApplicationType1 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Description { get; set; }

    [Column("notifyemails")]
    [StringLength(1000)]
    [Unicode(false)]
    public string Notifyemails { get; set; }

    public int? AdminPermissionId { get; set; }

    public enums.GroupType GroupType { get; set; }
    public int? ReportType { get; set; }

    [ForeignKey("ApplicationTypeId")]
    [InverseProperty("ApplicationTypes")]
    public virtual ICollection<ApplicationSection> ApplicationSections { get; } = new List<ApplicationSection>();
}
