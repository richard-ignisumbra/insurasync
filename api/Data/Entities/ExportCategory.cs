using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("ExportCategory")]
public partial class ExportCategory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("exportCategory")]
    [StringLength(100)]
    [Unicode(false)]
    public string ExportCategory1 { get; set; }

    public int? SortIndex { get; set; }
}
