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
    [Route("api/Categories")]
    public class CategoriesController : ControllerBase
    {

        const string scopeRequiredByAPI = "access_as_perma_user";
        private ICategoriesService categoriesService { get; set; }
        public CategoriesController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;

        }

        [HttpGet]
        [Route("NoteCategories")]
        public ActionResult<IList<models.NoteCategory>> GetNoteCategories()
        {
            if (categoriesService == null)
            {
                return NotFound();
            }
            var result = categoriesService.GetNoteCategories().ToList();
            return result;
        }
        [HttpGet]
        [Route("AttachmentCategories")]
        public ActionResult<IList<models.AttachmentCategory>> GetAttachmentCategories()
        {
            if (categoriesService == null)
            {
                return NotFound();
            }
            var result = categoriesService.GetAttachmentCategories().ToList();
            return result;
        }

    }

}