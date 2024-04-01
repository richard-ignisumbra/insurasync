using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph;

namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/Members")]
    public class MembersController : ControllerBase
    {
        private IMembersService _membersService;
        private IContactsService _contactService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public MembersController(IMembersService membersService, IContactsService contactService)
        {
            _membersService = membersService;
            _contactService = contactService;
        }
        [HttpPost]
        public ActionResult<int> AddMember(Member member)
        {
            if (_membersService == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_contactService.isUserAdmin(userId.Value) == false)
            {
                throw new InvalidOperationException();
            }
            var result = _membersService.AddMember(member, userId.Value);
            return result;
        }

        [HttpPut]
        public ActionResult<bool> UpdateMember(MemberDetails member)
        {
            if (_membersService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            var result = _membersService.UpdateMemberDetails(member, userId.Value);
            return result;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetAllMembers()
        {
            if (_membersService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            var result = _membersService.GetAllMembers(userId.Value).ToList();
            return result;
        }

        [HttpGet]
        [Route("{Id}")]
        public ActionResult<MemberDetails> GetMembers(int Id)
        {
            if (_membersService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _membersService.GetMemberDetails(Id, userId.Value);
            return result;
        }
        [HttpGet]
        [Route("{memberId}/allnotes")]
        public IEnumerable<models.members.noteSummary> GetMemberNotes(int memberId, string status)
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_contactService.isUserAdmin(userId.Value) == false)
            {
                throw new InvalidOperationException();
            }
            IEnumerable<models.members.noteSummary> result = _membersService.GetAllNotes(memberId, status);
            return result;
        }

        [HttpGet]
        [Route("{memberId}/allAttachments")]
        public IEnumerable<models.members.Attachment> GetMemberAttachments(int memberId, string status)
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_contactService.isUserAdmin(userId.Value) == false)
            {
                throw new InvalidOperationException();
            }
            IEnumerable<models.members.Attachment> result = _membersService.GetMemberAttachments(memberId, status);
            return result;
        }

        [HttpGet]
        [Route("{memberId}/notes/{noteId}")]
        public ActionResult<models.members.note> GetMemberNote(int memberId, int noteId)
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
            var result = _membersService.GetNote(memberId, noteId);
            return result;
        }
        [HttpPost]
        [Route("{memberId}/notes")]
        public ActionResult<int> SaveMemberNote(int memberId, models.members.note note)
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
            var result = _membersService.SaveNote(memberId, note, userId.Value);
            return result;
        }
        [HttpGet]
        [Route("{Id}/contacts")]
        public ActionResult<IEnumerable<Contact>> GetMemberContacts(int Id)
        {
            if (_contactService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _contactService.GetMemberContacts(Id, userId.Value).ToList();
            return result;
        }
        [HttpDelete]
        [Route("{Id}/contacts/{contactId}")]
        public ActionResult<Boolean> DeleteMemberFromContacts(int Id, int contactId)
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
            var result = _contactService.DeleteContactFromMember(contactId, Id);
            return result;
        }
        [HttpPost]
        [Route("{Id}/contacts/{contactId}/makePrimary")]
        public ActionResult<Boolean> MakeMemberPrimary(int Id, int contactId)
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
            var result = _contactService.MakeContactprimary(contactId, Id);
            return true;
        }
        [HttpPost]
        [Route("{Id}/contact")]
        public ActionResult<Boolean> AddContact2Member(int Id, AddContact addContact)
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
            var result = _contactService.AddContactToMember(addContact.ContactId, Id);
            return true;
        }
    }
}