using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

[Table("ApplicationSectionElement")]
public partial class ApplicationSectionElement
{
    [Key]
    public int ElementId { get; set; }

    public int? SectionId { get; set; }

    [StringLength(2000)]
    [Unicode(false)]
    public string LongText { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ShortName { get; set; }

    public int? ElementType { get; set; }

    public int? TableSectionId { get; set; }

    public int? DecimalPrecision { get; set; }

    [Column("isRequired")]
    public bool IsRequired { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Label { get; set; }

    public int? SortOrder { get; set; }

    [Column("sourceLabels")]
    [StringLength(2000)]
    [Unicode(false)]
    public string SourceLabels { get; set; }

    [Column("sourceValues")]
    [StringLength(2000)]
    [Unicode(false)]
    public string SourceValues { get; set; }

    [Column("maxLength")]
    public int? MaxLength { get; set; }

    [Column("originalSource")]
    [StringLength(150)]
    [Unicode(false)]
    public string OriginalSource { get; set; }

    [Column("allowNewRows")]
    public bool? AllowNewRows { get; set; }

    [Column("width")]
    public int? Width { get; set; }

    [Column("showAllLines")]
    public bool? ShowAllLines { get; set; }

    [Column("indentSpaces")]
    public int? IndentSpaces { get; set; }

    [Column("hideFromExport")]
    public bool? HideFromExport { get; set; }

    [Column("exportCategoryId")]
    public int? ExportCategoryId { get; set; }

    [Column("sumValues")]
    public bool? SumValues { get; set; }

    [InverseProperty("Element")]
    public virtual ICollection<ApplicationElementResponse> ApplicationElementResponses { get; } = new List<ApplicationElementResponse>();

    [ForeignKey("ElementId")]
    [InverseProperty("Elements")]
    public virtual ICollection<LineofCoverage> Lines { get; } = new List<LineofCoverage>();
}
