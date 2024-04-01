using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/ApplicationSectionElements")]
    public class ApplicationSectionElementsController : ControllerBase
    {
        private services.IApplicationElementsService _applicationSectionElementsService;
        private IApplicationsService _applicationService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public ApplicationSectionElementsController(services.IApplicationElementsService applicationSectionElementsService, IApplicationsService applicationService)
        {
            _applicationSectionElementsService = applicationSectionElementsService;
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("{applicationId}/section/${sectionId}")]
        public ActionResult<IEnumerable<models.applications.ApplicationElement>> GetApplicationSections(int applicationId, int sectionId)
        {
            if (_applicationSectionElementsService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var applicationDetails = _applicationService.GetApplication(applicationId, userId.Value);
            if (applicationDetails == null)
            {
                throw new Exception("invalid user access");
            }
            var results = _applicationSectionElementsService.GetApplicationElements(sectionId, applicationId).ToList();

            var elementResponses = _applicationSectionElementsService.GetResponses(sectionId, applicationId);
            foreach (var item in results)
            {
                item.Responses = (from response in elementResponses where response.ElementId == item.ElementId select response).ToList();
            }
            if (applicationDetails.PreviousApplicationId.HasValue)
            {
                var previousElementResponses = _applicationSectionElementsService.GetResponses(sectionId, applicationDetails.PreviousApplicationId.Value);
                foreach (var item in results)
                {
                    item.PreviousResponses = (from response in previousElementResponses where response.ElementId == item.ElementId select response).ToList();
                }
            }

            return results;
        }

        [HttpPost]
        [Route("{applicationId}/section/${sectionId}/${elementId}")]
        public ActionResult<enums.ElementSaveResponse> SaveElement(models.applications.ApplicationElementResponse response, int applicationId, int sectionId, int elementId)
        {


            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            response.ElementId = elementId;
            response.ApplicationId = applicationId;
            _applicationSectionElementsService.SaveApplicationResponse(response, applicationId, userId.Value);
            return enums.ElementSaveResponse.Success;
        }

    }

}