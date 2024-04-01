using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using permaAPI.models.applications;
using static System.Net.Mime.MediaTypeNames;
using permaAPI.services;
using ClosedXML.Excel;
using System.Net.Http;
using System.Net;
using permaAPI.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace permaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RequiredScope(scopeRequiredByAPI)]
    [Route("api/ApplicationTypes")]
    public class ApplicationTypesController : ControllerBase
    {
        private IApplicationTypeService _applicationTypeService;
        private ApplicationDbContext _context;
        private EmailService _emailService;
        const string scopeRequiredByAPI = "access_as_perma_user";

        public ApplicationTypesController(IApplicationTypeService applicationTypeService, EmailService emailService, ApplicationDbContext context)
        {
            _applicationTypeService = applicationTypeService;
            _emailService = emailService;
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<models.applications.ApplicationType>> GetApplicationTypes()
        {
            if (_applicationTypeService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);


            if (_context == null)
            {
                return NotFound();
            }

            var applicationTypes = _context.ApplicationTypes
                .Select(at => new models.applications.ApplicationType
                {
                    ApplicationTypeId = at.ApplicationTypeId,

                    Type = at.ApplicationType1,
                    Description = at.Description,
                    GroupType = at.GroupType,
                    ReportType = at.ReportType
                })
                .ToList();

            return applicationTypes;

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<models.applications.ApplicationType>> GetApplicationTypeById(int id)
        {
            if (_applicationTypeService == null)
            {
                return NotFound();
            }

            if (_context == null)
            {
                return NotFound();
            }

            var applicationType = await _context.ApplicationTypes
                .Select(at => new models.applications.ApplicationType
                {
                    ApplicationTypeId = at.ApplicationTypeId,
                    Type = at.ApplicationType1,
                    Description = at.Description,
                    GroupType = at.GroupType,
                    ReportType = at.ReportType
                })
                .FirstOrDefaultAsync(at => at.ApplicationTypeId == id);

            if (applicationType == null)
            {
                return NotFound();
            }

            return applicationType;
        }



        [HttpGet]
        [Route("reportTypes")]
        public ActionResult<IEnumerable<models.applications.ApplicationReportType>> GetApplicationReportTypes()
        {
            if (_applicationTypeService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (_context == null)
            {
                return NotFound();
            }

            var applicationReportTypes = _context.ReportTypes
                .Select(at => new models.applications.ApplicationReportType
                {
                    ReportType = at.ReportType1,

                    Name = at.Name,
                    Description = at.Description
                })
                .ToList();

            return applicationReportTypes;
        }

        [HttpGet]
        [Route("reports")]
        public ActionResult<IEnumerable<models.applications.ApplicationReport>> GetApplicationReports(int reportTypeId)
        {
            if (_applicationTypeService == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = _applicationTypeService.GetApplicationReports(reportTypeId).ToList();
            return result;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicationType(int id, models.applications.ApplicationType applicationType)
        {
            if (id != applicationType.ApplicationTypeId)
            {
                return BadRequest();
            }

            if (_context == null)
            {
                return NotFound();
            }

            _context.Entry(applicationType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ApplicationTypeExists(int id)
        {
            return _context.ApplicationTypes.Any(at => at.ApplicationTypeId == id);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchApplicationType(int id, JsonPatchDocument<Data.Entities.ApplicationType> patchDoc)
        {
            if (_context == null)
            {
                return NotFound();
            }

            var applicationType = await _context.ApplicationTypes.FindAsync(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(applicationType, (error) =>
            {
                // Handle the error
                ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }



    }

}