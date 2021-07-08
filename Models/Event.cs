using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Event
    {
        [Key]
        [Required]
        public int EventId { get; set; }

        [Required]
        [Display(Name ="Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name ="Date")]
        public DateTime EventDate {get; set;}

        [Required]
        [DataType(DataType.Time)]
        [Display(Name ="Time")]
        public DateTime EventTime {get; set;}

        [Required]
        [Display(Name ="Duration")]
        public string Duration { get; set; }

        [Required]
        public string DurationUnit {get; set;}

        [Required]
        [Display(Name ="Description")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Foreign Key for UserId has to match the property name in User class
        public int UserId { get; set; }
        public User Coordainator { get; set; }
        public List<RSVP> Participants { get; set; }


    }
}