using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Keyless]
[Table("InsuredNote")]
public partial class InsuredNote
{
    [Column("categoryId")]
    public int CategoryId { get; set; }

    [Required]
    [Column("subject")]
    [StringLength(500)]
    [Unicode(false)]
    public string Subject { get; set; }

    [Required]
    [Column("content")]
    [Unicode(false)]
    public string Content { get; set; }

    [Required]
    [Column("status")]
    [StringLength(50)]
    [Unicode(false)]
    public string Status { get; set; }

    [Required]
    [Column("createdBy")]
    [StringLength(150)]
    [Unicode(false)]
    public string CreatedBy { get; set; }

    [Column("createdDate", TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column("memberId")]
    public int MemberId { get; set; }

    [Column("noteId")]
    public int NoteId { get; set; }

    [Required]
    [Column("applicationUser")]
    [StringLength(500)]
    [Unicode(false)]
    public string ApplicationUser { get; set; }

    [Required]
    [Column("modifiedBy")]
    [StringLength(150)]
    [Unicode(false)]
    public string ModifiedBy { get; set; }

    [Column("modifiedDate", TypeName = "datetime")]
    public DateTime ModifiedDate { get; set; }
}
