using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[PrimaryKey("MemberId", "ContactId")]
[Table("InsuredContact")]
public partial class InsuredContact
{
    [Key]
    public int MemberId { get; set; }

    [Key]
    public int ContactId { get; set; }

    [Column("isDeleted")]
    public bool? IsDeleted { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("InsuredContacts")]
    public virtual Contact Member { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("InsuredContacts")]
    public virtual Insured MemberNavigation { get; set; }
}
