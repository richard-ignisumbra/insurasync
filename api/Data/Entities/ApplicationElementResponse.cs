using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[PrimaryKey("ApplicationId", "ElementId", "RowId")]
[Table("ApplicationElementResponse")]
public partial class ApplicationElementResponse
{
    [Key]
    public int ApplicationId { get; set; }

    [Key]
    public int ElementId { get; set; }

    [Key]
    public int RowId { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal? CurrencyResponse { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string TextResponse { get; set; }

    [Unicode(false)]
    public string LongTextResponse { get; set; }

    public int? IntResponse { get; set; }

    public bool? BitResponse { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateResponse { get; set; }

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

    [Column("numRows")]
    public int? NumRows { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("ApplicationElementResponses")]
    public virtual Application Application { get; set; }

    [ForeignKey("ElementId")]
    [InverseProperty("ApplicationElementResponses")]
    public virtual ApplicationSectionElement Element { get; set; }
}
