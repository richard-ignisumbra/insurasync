using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[PrimaryKey("ApplicationId", "ElementId")]
[Table("ApplicationElementLabel")]
public partial class ApplicationElementLabel
{
    [Key]
    public int ApplicationId { get; set; }

    [Key]
    public int ElementId { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Label { get; set; }
}
