using System;
namespace permaAPI.models
{
    public class UserProfile
    {
        public UserProfile()
        { }
        public Boolean isAdmin { get; set; }
        public System.Collections.Generic.IList<int> MemberIds { get; set; }

    }
}

