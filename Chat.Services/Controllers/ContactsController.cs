﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Chat.DataLayer;
using Chat.Repositories;
using Chat.Services.Models;
using Forum.WebApi.Attributes;

namespace Chat.Services.Controllers
{
    public class ContactsController : ApiController
    {
        private UsersRepository usersRepository;

        public ContactsController()
        {
            var context = new ChatDatabaseContext();
            this.usersRepository = new UsersRepository(context);
        }

        [HttpGet]
        [ActionName("all")]
        public HttpResponseMessage GetContacts(
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
        [ActionName("add")]
        public HttpResponseMessage SendContactRequest(int id, 
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersRepository.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var receiver = usersRepository.Get(id);

            if(usersRepository.SendContactRequest(sender, receiver))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          "You have already sent request to this person");
        }

        [HttpGet]
        [ActionName("accept")]
        public HttpResponseMessage AcceptContactRequest(int id, 
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersRepository.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            if(usersRepository.AcceptContactRequest(id, user))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          "Wrong contact request");
        }

        [HttpGet]
        [ActionName("deny")]
        public HttpResponseMessage DenyContactRequest(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersRepository.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            if (usersRepository.DenyContactRequest(id, user))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          "Wrong contact request");
        }
    }
}