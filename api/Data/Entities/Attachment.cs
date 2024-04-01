using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("Attachment")]
public partial class Attachment
{
    [Key]
    [Column("attachmentId")]
    public int AttachmentId { get; set; }

    [Column("memberId")]
    public int MemberId { get; set; }

    [Column("categoryId")]
    public int CategoryId { get; set; }

    [Column("policyPeriod")]
    public int PolicyPeriod { get; set; }

    [Required]
    [Column("createdBy")]
    [StringLength(500)]
    [Unicode(false)]
    public string CreatedBy { get; set; }

    [Required]
    [Column("fileName")]
    [StringLength(500)]
    [Unicode(false)]
    public string FileName { get; set; }

    [Column("createdDate", TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [Column("status")]
    [StringLength(50)]
    [Unicode(false)]
    public string Status { get; set; }

    [Required]
    [Column("applicationUser")]
    [StringLength(500)]
    [Unicode(false)]
    public string ApplicationUser { get; set; }

    [Column("modifiedDate", TypeName = "datetime")]
    public DateTime ModifiedDate { get; set; }

    [Required]
    [Column("modifiedBy")]
    [StringLength(500)]
    [Unicode(false)]
    public string ModifiedBy { get; set; }

    [Required]
    [Unicode(false)]
    public string FileDescription { get; set; }
}
