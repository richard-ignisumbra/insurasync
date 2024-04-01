using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/ApplicationSections")]
    public class ApplicationSectionsController : ControllerBase
    {
        private services.IApplicationSectionService _applicationSectionService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public ApplicationSectionsController(services.IApplicationSectionService applicationSectionService)
        {
            _applicationSectionService = applicationSectionService;
        }

        [HttpGet]
        [Route("{applicationId}")]
        public ActionResult<IEnumerable<models.applications.ApplicationSection>> GetApplicationSections(int applicationId)
        {
            if (_applicationSectionService == null)
            {
                return NotFound();
            }
            var result = _applicationSectionService.GetApplicationSections(applicationId).ToList();
            return result;
        }

        [HttpPost]
        [Route("{applicationId}/{sectionId}/complete")]
        public ActionResult<Boolean> PostCompleteSection(int applicationId, int sectionId, Boolean isCompleted)
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_applicationSectionService == null)
            {
                return NotFound();
            }
            var result = _applicationSectionService.CompleteSection(applicationId, sectionId, isCompleted);
            return result;
        }
    }

}