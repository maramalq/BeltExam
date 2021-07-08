using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class RSVP
    {
        [Key]
        [Required]
        public int RsvpId { get; set; }

        // bring foriegn keys for all users and events
        public int UserId { get; set; }
        public User Attendee { get; set; }
        public int EventId { get; set; }
        public Event AttendeeOf { get; set; }
        // public Wedding AttendOf { get; set; }
    }
}