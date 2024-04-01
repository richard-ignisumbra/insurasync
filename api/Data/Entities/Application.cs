using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("Application")]
public partial class Application
{
    [Key]
    public int ApplicationId { get; set; }

    public int? ApplicationTypeId { get; set; }

    public int MemberId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string ApplicationName { get; set; }

    public int? CoverageYear { get; set; }

    public int? PeriodQuarter { get; set; }
    public int? PeriodMonth { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ApplicationStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CompletedDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CompletedBy { get; set; }

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

    [Column("previousApplicationId")]
    public int? PreviousApplicationId { get; set; }

    [InverseProperty("Application")]
    public virtual ICollection<ApplicationElementResponse> ApplicationElementResponses { get; } = new List<ApplicationElementResponse>();

    [InverseProperty("Application")]
    public virtual ICollection<ApplicationSectionResponse> ApplicationSectionResponses { get; } = new List<ApplicationSectionResponse>();
}
