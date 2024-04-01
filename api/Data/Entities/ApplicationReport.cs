using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("ApplicationReport")]
public partial class ApplicationReport
{
    [Key]
    [Column("reportId")]
    public int ReportId { get; set; }

    [Required]
    [Column("reportName")]
    [StringLength(150)]
    [Unicode(false)]
    public string ReportName { get; set; }

    [Required]
    [Column("reportDescription")]
    [StringLength(500)]
    [Unicode(false)]
    public string ReportDescription { get; set; }

    [Column("reportTypeId")]
    public int ReportTypeId { get; set; }

    [Column("reportAction")]
    [StringLength(100)]
    [Unicode(false)]
    public string ReportAction { get; set; }
}
