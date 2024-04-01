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
    [Route("api/Lines")]
    public class LinesController : ControllerBase
    {
        private ILinesService _linesService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public LinesController(ILinesService linesService)
        {
            _linesService = linesService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IList<string>> GetLines()
        {
            if (_linesService == null)
            {
                return NotFound();
            }
            var result = _linesService.GetLines().ToList();
            return result;
        }
    }

}