
using System;
using System.ComponentModel.DataAnnotations;
namespace permaAPI.models.users
{
    public class UserInvite
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}

