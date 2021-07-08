using System.Collections.Generic;

namespace BeltExam.Models
{
    public class Container
    {
        public User User { get; set; }
        public List<User> Users { get; set; }
        public Event Event { get; set; }
        public List<Event> Events { get; set; }
        // public Message Message { get; set; }
        // public List<Message> Messages { get; set; }
        // public Comment Comment { get; set; }
        // public List<Comment> Comments { get; set; }
    }
}
