using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assig1.Data;
using Assig1.Models;
using static System.Collections.Specialized.BitVector32;
using Assig1.ViewModels;

namespace Assig1.Controllers
{
    public class OffencesController : Controller
    {
        private readonly ExpiationsContext _context;

        public OffencesController(ExpiationsContext context)
        {
            _context = context;
        }

        // GET: Offences
        public async Task<IActionResult> Index(string offence, int? categoryId)
        {
            ViewBag.Title = "Offences";
            ViewBag.Active = "Offences";

            var expiationsContext = from Offence in _context.Offences
                                    join Section in _context.Sections on Offence.SectionId equals Section.SectionId
                                    join ExpiationCategory in _context.ExpiationCategories on Section.CategoryId equals ExpiationCategory.CategoryId
                                    select new OffenceAndExpiationCategory
                                    {
                                        OffenceCode = Offence.OffenceCode,
                                        Description = Offence.Description,
                                        ExpiationFee = Offence.ExpiationFee,
                                        AdultLevy = Offence.AdultLevy,
                                        CorporateFee = Offence.CorporateFee,
                                        TotalFee = Offence.TotalFee,
                                        DemeritPoints = Offence.DemeritPoints,
                                        SectionId = Section.SectionId,
                                        SectionCode = Section.SectionCode,
                                        CategoryId = ExpiationCategory.CategoryId,
                                        CategoryName = ExpiationCategory.CategoryName,
                                        ParentCategoryId = ExpiationCategory.ParentCategoryId
                                    };
            #region Categories
            if (categoryId != null)
            {
                var category = expiationsContext
                    .Where(ec => ec.CategoryId == categoryId);
                expiationsContext = category;
            }

            #endregion 

            #region SearchFunction

            if (!string.IsNullOrEmpty(offence))
            {
                var search = expiationsContext
                    .Where(ec => ec.Description.Contains(offence));
                expiationsContext = search;
            }

            #endregion 

            return View(await expiationsContext.ToListAsync());
        }

        [Route("OffenceController/GetCategories")]
        public IActionResult GetCategories()
        {
            var categories = _context.ExpiationCategories
                .Distinct()
                .Select(ec => new
                {
                    ec.CategoryId,
                    ec.CategoryName,
                })
                .ToList();

            return Json(categories);
        }

        [Route("OffenceController/GetOffenceDescriptions")]
        public IActionResult GetOffenceDescriptions(string search)
        {
            var results = _context.Offences
                .Where(o => o.Description.Contains(search))
                .OrderBy(o => o.Description)
                .Select(o => o.Description)
                .Take(10)
                .ToList();

            return Json(results);
        }



        // GET: Offences/Details/A002
        public async Task<IActionResult> Details(string id)
        {
            ViewBag.Title = "Offences";
            ViewBag.Active = "Offences";
            if (id == null)
            {
                return NotFound();
            }

            var offence = await _context.Offences
                .Include(o => o.Section)
                .Include(o => o.Expiations)
                .FirstOrDefaultAsync(m => m.OffenceCode == id);

            if (offence != null)
            {
                var expiationsList = offence.Expiations;
                
                var offenceAndExpiationViewModel = expiationsList
                    .Select(o => new OffenceAndExpiation {
                        IncidentStartDate = o.IncidentStartDate,
                        VehicleSpeed = o.VehicleSpeed,
                        LocationSpeedLimit = o.LocationSpeedLimit,
                        DriverState = o.DriverState,
                        TotalFeeAmt = o.TotalFeeAmt,
                    })
                    .ToList();
            }


            if (offence == null)
            {
                return NotFound();
            }

            return View(offence);
        }


        private bool OffenceExists(string id)
        {
            return _context.Offences.Any(e => e.OffenceCode == id);
        }
    }
}
