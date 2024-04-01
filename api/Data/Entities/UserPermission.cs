using System;
namespace permaAPI.Data.Entities
{
    public class UserPermission
    {
        public int PermissionId { get; set; }
        public int ContactId { get; set; }

        // Navigation properties
        public PermissionType PermissionType { get; set; }
        public Contact Contact { get; set; }
    }
}

