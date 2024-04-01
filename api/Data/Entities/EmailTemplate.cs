using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("emailTemplate")]
public partial class EmailTemplate
{
    [Key]
    [Column("templatetype")]
    [StringLength(50)]
    [Unicode(false)]
    public string Templatetype { get; set; }

    [Column("emailTemplateBody")]
    [Unicode(false)]
    public string EmailTemplateBody { get; set; }

    [Column("emailSubject")]
    [StringLength(500)]
    [Unicode(false)]
    public string EmailSubject { get; set; }

    [Column("recipients")]
    [Unicode(false)]
    public string Recipients { get; set; }
}
