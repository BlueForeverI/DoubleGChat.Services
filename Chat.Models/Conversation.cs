﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public virtual User FirstUser { get; set; }
        public virtual User SecondUser { get; set; }

        public virtual ICollection<Message> Messages { get; set; } 

        public Conversation()
        {
            this.Messages = new HashSet<Message>();
        }
    }
}
