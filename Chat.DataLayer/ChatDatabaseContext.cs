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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Contacts)
                .WithMany(u => u.Contacts).Map(map =>
                                                   {
                                                       map.ToTable("UsersUsers");
                                                       map.MapLeftKey("FirstUserId");
                                                       map.MapRightKey("SecondUserId");
                                                   });

            base.OnModelCreating(modelBuilder);
        }
    }
}
