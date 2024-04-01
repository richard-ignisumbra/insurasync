using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace permaAPI.Data.Entities;

public partial class ConfigurationSetting
{
    [Key]
    [StringLength(100)]
    [Unicode(false)]
    public string ConfigurationKey { get; set; }

    [Unicode(false)]
    public string ConfigurationValue { get; set; }
}
