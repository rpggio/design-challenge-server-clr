using System;
using System.IO;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using DCS.Core.Validation;
using DCS.ServerRuntime.Entities;
using Nancy;
using Nancy.ModelBinding;

namespace DCS.WebServices.Api
{
    public class ContactsEndpoint : NancyModule
    {
        public ContactsEndpoint(EntityStore<ContactEntity, int> contacts)
            : base("/contacts")
        {
            Get["/"] = parameters =>
            {
                string token = Request.Query.token;
                if (token.ToNullableGuid() != FakeyAuth.Token)
                {
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }

                return contacts.GetAll();
            };

            Post["/"] = parameters =>
            {
                var contact = this.Bind<ContactEntity>();
                contact.Created = DateTime.UtcNow;
                ValidationUtil.AssertIsValid(contact);
                contacts.Save(contact);
                return contact;
            };
        }
    }
}