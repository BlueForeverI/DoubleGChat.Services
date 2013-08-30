﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Models;

namespace Chat.DataLayer
{
    public class ChatDatabaseContext : DbContext
    {
        public ChatDatabaseContext() : base("DoubleGChatDatabase")
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }
    }
}
