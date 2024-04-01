using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("NoteCategory")]
public partial class NoteCategory
{
    [Key]
    [Column("categoryId")]
    public int CategoryId { get; set; }

    [Required]
    [Column("categoryTitle")]
    [StringLength(100)]
    [Unicode(false)]
    public string CategoryTitle { get; set; }
}
