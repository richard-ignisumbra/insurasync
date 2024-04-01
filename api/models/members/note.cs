using System;
namespace permaAPI.models.members
{
    public class noteSummary
    {
        public int? NoteId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string Subject { get; set; }

        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public noteSummary()
        {
        }
    }

    public class note : noteSummary

    {

        public string Content { get; set; }


        public note()
        {
        }


    }
}

