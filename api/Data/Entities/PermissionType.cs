using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("permissionType")]
public partial class PermissionType
{
    [Key]
    [Column("permissionId")]
    public int PermissionId { get; set; }

    [Column("permissionName")]
    [StringLength(100)]
    [Unicode(false)]
    public string PermissionName { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("Permissions")]
    public virtual ICollection<Contact> Contacts { get; } = new List<Contact>();
}
