using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("ReportType")]
public partial class ReportType
{
    [Key]
    [Column("reportType")]
    public int ReportType1 { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string Description { get; set; }
}
