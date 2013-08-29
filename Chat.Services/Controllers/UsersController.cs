﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Chat.DataLayer;
using Chat.Models;
using Chat.Repositories;
using Chat.Services.Models;
using Forum.WebApi.Attributes;

namespace Chat.Services.Controllers
{
    public class UsersController : ApiController
    {
        private UsersRepository usersRepository;
        private const int SessionKeyLength = 50;
        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        private static readonly Random rand = new Random();

        public UsersController()
        {
            var context = new ChatDatabaseContext();
            this.usersRepository = new UsersRepository(context);
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<User> All()
        {
            return usersRepository.All();
        }

        [HttpGet]
        [ActionName("contacts")]
        public HttpResponseMessage Friends(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersRepository.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var contacts = user.Contacts.Select(u => new UserModel()
                                                         {
                                                             Username = u.Username,
                                                             FirstName = u.FirstName,
                                                             LastName = u.LastName,
                                                             ProfilePictureUrl = u.ProfilePictureUrl
                                                         });

            return Request.CreateResponse(HttpStatusCode.OK, contacts);
        }

        [HttpGet]
        [ActionName("byid")]
        public User GetById(int id)
        {
            return usersRepository.Get(id);
        }

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage Register([FromBody]User value)
        {
            if(string.IsNullOrEmpty(value.Username) || string.IsNullOrWhiteSpace(value.Username)
                || value.Username.Length < 5 || value.Username.Length > 30)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              "Invalid username. Should be between 5 and 30 characters");
            }

            if(usersRepository.GetByUsername(value.Username) != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              "Username already exists");
            }

            usersRepository.Add(value);
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage Login([FromBody]User value)
        {
            var user = usersRepository.CheckLogin(value.Username, value.PasswordHash);
            if (user != null)
            {
                var sessionKey = GenerateSessionKey(user.Id);
                usersRepository.SetSessionKey(user, sessionKey);

                var userModel = new UserLoggedModel() {SessionKey = sessionKey, Username = user.Username};
                return Request.CreateResponse(HttpStatusCode.OK, userModel);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
            }
        }

        [HttpGet]
        [ActionName("logout")]
        public HttpResponseMessage Logout(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersRepository.GetBySessionKey(sessionKey);
            if(user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            usersRepository.Logout(user);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("byusername")]
        public User GetByUsername([FromBody]User userData)
        {
            var user = usersRepository.GetByUsername(userData.Username);
            return user;
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }
    }
}
