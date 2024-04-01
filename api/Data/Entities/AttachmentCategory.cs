using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("AttachmentCategory")]
public partial class AttachmentCategory
{
    [Required]
    [StringLength(150)]
    [Unicode(false)]
    public string CategoryTitle { get; set; }

    [Key]
    public int CategoryId { get; set; }
}
