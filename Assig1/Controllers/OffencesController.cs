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

        [Route("OffenceController/GetCategoryOffences")]
        public IActionResult GetCategoryOffences()
        {
            var offenceCategories = from Offence in _context.Offences
                                    join Section in _context.Sections on Offence.SectionId equals Section.SectionId
                                    join ExpiationCategory in _context.ExpiationCategories on Section.CategoryId equals ExpiationCategory.CategoryId
                                    select new
                                    {
                                        Offence = Offence.OffenceCode,
                                        CategoryName = ExpiationCategory.CategoryName,

                                    };

            var results = offenceCategories
                .GroupBy(oc => new { oc.CategoryName })
                .Select(g => new {
                    CategoryName = g.Key,
                    OffenceCount = g.Count()
                })
                .ToList();



            return Json(results);
        }

        // GET: Offences/Details/A002
        public async Task<IActionResult> Details(string id, DateOnly? beforeDate, DateOnly? afterDate, int? minFee, int? maxFee)
        {
            ViewBag.Title = "Offences";
            ViewBag.Active = "Offences";
            if (id == null)
            {
                return NotFound();
            }

           OffenceExpiationDetails offenceExpiationDetails = new OffenceExpiationDetails();

            var offence = await _context.Offences
                .Include(o => o.Section)
                .Include(o => o.Expiations)
                .FirstOrDefaultAsync(m => m.OffenceCode == id);

            offenceExpiationDetails.Offence = offence;

           if (beforeDate == null && afterDate == null && minFee == null && maxFee == null)
            {
                offenceExpiationDetails.OffenceAndExpiations = null;
            }
            else
            {
                List<OffenceAndExpiation> expiations = null;

                if (offence != null)
                {
                    var expiationsList = offence.Expiations;

                    var offenceAndExpiationViewModel = expiationsList
                        .Select(o => new OffenceAndExpiation
                        {
                            ExpId = o.ExpId,
                            IncidentStartDate = o.IncidentStartDate,
                            VehicleSpeed = o.VehicleSpeed,
                            LocationSpeedLimit = o.LocationSpeedLimit,
                            DriverState = o.DriverState,
                            TotalFeeAmt = o.TotalFeeAmt,
                            Offence = offence
                        })
                        .ToList();

                    expiations = offenceAndExpiationViewModel;
                }
                else
                {
                    return NotFound();
                }

                if (beforeDate != null)
                {
                    var beforeDateQuery = expiations
                        .Where(e => e.IncidentStartDate <= beforeDate)
                        .ToList();

                    expiations = beforeDateQuery;
                }

                if (afterDate != null)
                {
                    var afterDateQuery = expiations
                        .Where(e => e.IncidentStartDate >= afterDate)
                        .ToList();

                    expiations = afterDateQuery;
                }

                if (minFee != null)
                {
                    var minFeeQuery = expiations
                        .Where(e => e.TotalFeeAmt >= minFee)
                        .ToList();

                    expiations = minFeeQuery;
                }

                if (maxFee != null)
                {
                    var maxFeeQuery = expiations
                        .Where(e => e.TotalFeeAmt <= maxFee)
                        .ToList();

                    expiations = maxFeeQuery;
                }

                offenceExpiationDetails.OffenceAndExpiations = expiations;
            }


            return View(offenceExpiationDetails);
        }

        [Route("OffenceController/GetTotalFeePerMonth")]
        public async Task<IActionResult> GetTotalFeePerMonth(string id)
        {
            var offence = await _context.Offences
               .Include(o => o.Section)
               .Include(o => o.Expiations)
               .FirstOrDefaultAsync(m => m.OffenceCode == id);

            var expiationsList = offence.Expiations;

            var totalFeePerMonth = expiationsList
                .GroupBy(o => new { o.IncidentStartDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    TotalFees = g.Sum(o => o.TotalFeeAmt)
                })
                .ToList();


            return Json(totalFeePerMonth);
        }

        [Route("OffenceController/GetTotalOffencesPerMonth")]
        public async Task<IActionResult> GetTotalOffencesPerMonth(string id)
        {
            var offence = await _context.Offences
               .Include(o => o.Section)
               .Include(o => o.Expiations)
               .FirstOrDefaultAsync(m => m.OffenceCode == id);

            var expiationsList = offence.Expiations;

            var totalOffencesPerMonth = expiationsList
                .GroupBy(o => new { o.IncidentStartDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    TotalOffences = g.Count()
                })
                .ToList();


            return Json(totalOffencesPerMonth);
        }

        public async Task<IActionResult> MoreDetails(string id)
        {
            var offence = await _context.Offences
               .Include(o => o.Section)
               .Include(o => o.Expiations)
               .FirstOrDefaultAsync(m => m.OffenceCode == id);

            var expiationsList = offence.Expiations;

            var maxSpeed = expiationsList
                .Max(e => e.VehicleSpeed);

            var minSpeed = expiationsList
                .Min(e => e.VehicleSpeed);

            var avgSpeed = expiationsList
                .Average(e => e.VehicleSpeed);

            var offenceCode = id;


            ExpiationDetails expiationDetails = new ExpiationDetails();
            expiationDetails.MaxSpeed = maxSpeed;
            expiationDetails.MinSpeed = minSpeed;
            expiationDetails.AvgSpeed = avgSpeed;
            expiationDetails.OffenceCode = offenceCode;


            return View(expiationDetails);
        }

        private bool OffenceExists(string id)
        {
            return _context.Offences.Any(e => e.OffenceCode == id);
        }
    }
}
