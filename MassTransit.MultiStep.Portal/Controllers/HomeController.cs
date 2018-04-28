using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MassTransit.MultiStep.Portal.Models;
using MassTransit.MultiStep.Portal.Infrastructure;
using MassTransit.MultiStep.Common.EventMessages;

namespace MassTransit.MultiStep.Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventPublishingService _eventPublishingService;
        public HomeController(EventPublishingService eventPublishingService)
        {
            _eventPublishingService = eventPublishingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SubmitPolicyRequest()
        {
            await _eventPublishingService.Publish<IUnderwritingSubmissionSubmitted>(new UnderwritingSubmissionSubmitted()
            {
                SubmissionId = Guid.NewGuid(),
                SubmittedBy = "Ben"
            });

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
