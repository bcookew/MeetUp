using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetUp.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [FutureDate]
        public DateTime EventStartTime { get; set; }
        public DateTime EventEndTime { get; set; }


        [Required]
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ====================== Unmapped props for form/validation
        [NotMapped]
        public int Duration { get; set; }
        [NotMapped]
        public int TimeMeasure { get; set; }
        
        // ====================== Nav Properties
        public User Creator { get; set; }
        public List<ParticipantAtEvent> Participants { get; set; }
    }
    
}