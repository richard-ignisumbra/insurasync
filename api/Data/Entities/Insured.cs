using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("Insured")]
public partial class Insured
{
    [Key]
    public int MemberId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string MemberName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MemberStatus { get; set; }

    [Column("isDeleted")]
    public bool? IsDeleted { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string OrganizationType { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string CompanyName { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string Address1 { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string Address2 { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string City { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string State { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Zip { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Phone { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string Website { get; set; }

    public int? ParentMember { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string StateofIncorporation { get; set; }

    [Column("FEIN")]
    [StringLength(20)]
    [Unicode(false)]
    public string Fein { get; set; }

    public int? PrimaryContactId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    [Column(TypeName = "text")]
    public string Notes { get; set; }

    [Column(TypeName = "date")]
    public DateTime? JoinDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MemberNumber { get; set; }

    [InverseProperty("MemberNavigation")]
    public virtual ICollection<InsuredContact> InsuredContacts { get; } = new List<InsuredContact>();

    [ForeignKey("MemberId")]
    [InverseProperty("Members")]
    public virtual ICollection<LineofCoverage> LineofCoverages { get; } = new List<LineofCoverage>();
}
