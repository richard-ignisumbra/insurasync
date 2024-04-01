using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using permaAPI.services;

namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/Contacts")]
    public class ContactsController : ControllerBase
    {
        private IContactsService _contactService;
        private MsGraphService _msGraphService;
        private EmailService _emailService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public ContactsController(IContactsService contactService, MsGraphService msGraphService, EmailService emailService)
        {
            _contactService = contactService;
            _msGraphService = msGraphService;
            _emailService = emailService;
        }

        [HttpGet]
        [Route("profile")]
        public ActionResult<permaAPI.models.UserProfile> Getprofile()
        {
            if (_contactService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _contactService.GetUserProfile(userId.Value);

            return result;
        }
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<Contact>> GetContacts()
        {
            if (_contactService == null)
            {
                return NotFound();
            }
            var result = _contactService.GetAllContacts().ToList();
            return result;
        }

        [HttpGet]
        [Route("{Id}")]
        public ActionResult<ContactDetails> GetContact(int Id)
        {
            if (_contactService == null)
            {
                return NotFound();
            }
            var result = _contactService.GetContact(Id);
            return result;
        }
        [HttpPost]
        public ActionResult<int> AddContact(NewContact contact)
        {
            if (_contactService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _contactService.AddContact(contact, userId.Value);
            return result;
        }

        [HttpPut]
        public ActionResult<bool> UpdateContact(ContactDetails contact)
        {
            if (_contactService == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _contactService.UpdateContact(contact, userId.Value);
            return result;
        }

        [HttpPut]
        [Route("api/Contacts/{contactId}/makeadmin")]
        public async Task<ActionResult<Boolean>> MakeAdmin(int contactId)
        {
            if (_contactService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_contactService.isUserAdmin(userId.Value) == false)
            {
                throw new InvalidOperationException();
            }
            return _contactService.makeUserAdmin(contactId);


        }

        [HttpPut]
        [Route("api/Contacts/{contactId}/invite")]
        public async Task<ActionResult<InviteResult>> InviteUser(int contactId)
        {
            var inviteResult = new InviteResult();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_contactService.isUserAdmin(userId.Value) == false)
            {
                throw new InvalidOperationException();
            }

            var existingContact = this._contactService.GetContact(contactId);
            if (existingContact.isApplicationUser == false)
            {
                var newUser = new models.users.UserModelB2CIdentity();
                newUser.Email = existingContact.Email.Trim();
                newUser.DisplayName = existingContact.DisplayName;
                newUser.GivenName = existingContact.FirstName;
                newUser.Surname = existingContact.LastName;



                try
                {
                    var result = await this._msGraphService.CreateUserWithCustomDomain(newUser);
                    if (!String.IsNullOrEmpty(result.Id))
                    {
                        existingContact.userIdentifier = result.Id;
                        this._contactService.UpdateContact(existingContact, userId.Value);
                        inviteResult.NewPassword = result.Password;
                        inviteResult.userLogin = result.Upn;
                        inviteResult.isSuccessful = true;
                        var emailResult = await this._emailService.SendInviteEmail(existingContact, inviteResult.NewPassword);
                        if (emailResult != null & emailResult.IsSuccessStatusCode == false)
                        {
                            inviteResult.EmailResult = emailResult.StatusCode.ToString();
                        }

                    }
                    else
                    {
                        inviteResult.isSuccessful = false;
                        inviteResult.inviteResult = "user already member";

                    }
                }
                catch (Exception ex)
                {
                    inviteResult.isSuccessful = false;
                    inviteResult.inviteResult = "user invite error - " + ex.Message;
                }

            }
            else
            {
                inviteResult.isSuccessful = false;
                inviteResult.inviteResult = "user already member";

            }

            if (inviteResult.isSuccessful)
            {

            }

            return inviteResult;
        }

    }

    public class InviteResult
    {
        public string inviteResult { get; set; }
        public Boolean isSuccessful { get; set; }
        public string NewPassword { get; set; }
        public string userLogin { get; set; }
        public string EmailResult { get; set; }
    }
}