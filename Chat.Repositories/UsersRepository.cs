using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.DataLayer;
using Chat.Models;

namespace Chat.Repositories
{
    public class UsersRepository : EfRepository<User>
    {
        private ChatDatabaseContext chatContext;

        public UsersRepository(ChatDatabaseContext context) : base(context)
        {
            this.chatContext = context;
        }

        public User CheckLogin(string username, string passwordHash)
        {
            var user = chatContext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user;
        }

        public User GetByUsername(string username)
        {
            return chatContext.Users.FirstOrDefault(u => u.Username == username);
        }

        public User GetBySessionKey(string sessionKey)
        {
            return chatContext.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
        }

        public void Logout(User user)
        {
            chatContext.Users.Attach(user);
            user.SessionKey = null;
            chatContext.SaveChanges();
        }

        public void SetSessionKey(User user, string sessionKey)
        {
            chatContext.Users.Attach(user);
            user.SessionKey = sessionKey;
            chatContext.SaveChanges();
        }

        public bool SendContactRequest(User sender, User receiver)
        {
            chatContext.Users.Attach(sender);
            chatContext.Users.Attach(receiver);

            if(receiver.ContactRequests.Any(c => c.Sender.Id == sender.Id))
            {
                return false;
            }

            receiver.ContactRequests.Add(new ContactRequest(){Sender = sender});
            chatContext.SaveChanges();
            return true;
        }

        public bool AcceptContactRequest(int requestId, User user)
        {
            var request = chatContext.ContactRequests.FirstOrDefault(c => c.Id == requestId);
            if(request == null)
            {
                return false;
            }

            chatContext.Users.Attach(user);
            if(!user.ContactRequests.Any(c => c.Id == requestId))
            {
                return false;
            }

            user.Contacts.Add(request.Sender);
            request.Sender.Contacts.Add(user);
            chatContext.SaveChanges();

            chatContext.ContactRequests.Remove(request);
            chatContext.SaveChanges();

            return true;
        }

        public bool DenyContactRequest(int requestId, User user)
        {
            var request = chatContext.ContactRequests.FirstOrDefault(c => c.Id == requestId);
            if (request == null)
            {
                return false;
            }

            chatContext.Users.Attach(user);
            if (!user.ContactRequests.Any(c => c.Id == requestId))
            {
                return false;
            }

            chatContext.ContactRequests.Remove(request);
            chatContext.SaveChanges();

            return true;
        }

        public void SetOnline(User user, bool online)
        {
            chatContext.Users.Attach(user);
            user.Online = online;
            chatContext.SaveChanges();
        }
    }
}
