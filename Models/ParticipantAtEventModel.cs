using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetUp.Models
{
    public class ParticipantAtEvent
    {
        [Key]
        public int ParticipantAtEventId { get; set; }
        public int UserId { get; set; }
        public User Participant { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}