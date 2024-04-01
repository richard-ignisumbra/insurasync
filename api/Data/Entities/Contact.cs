using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("contact")]
public partial class Contact
{
    [Key]
    public int ContactId { get; set; }

    public int? InsuredId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Salutation { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FirstName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string MiddleName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string LastName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string JobTitle { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ContactType { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string BusinessPhone { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string HomePhone { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MobilePhone { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Fax { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string AddressType { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Address1 { get; set; }

    [StringLength(200)]
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

    [StringLength(1000)]
    [Unicode(false)]
    public string Description { get; set; }

    [Column("isBoardMember")]
    public bool? IsBoardMember { get; set; }

    [Column("isExecutiveCommitteeMember")]
    public bool? IsExecutiveCommitteeMember { get; set; }

    [Column("isDeleted")]
    public int? IsDeleted { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MemberType { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    [Column("userIdentifier")]
    [StringLength(1000)]
    [Unicode(false)]
    public string UserIdentifier { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<InsuredContact> InsuredContacts { get; } = new List<InsuredContact>();

    [ForeignKey("ContactId")]
    [InverseProperty("Contacts")]
    public virtual ICollection<PermissionType> Permissions { get; } = new List<PermissionType>();
}
