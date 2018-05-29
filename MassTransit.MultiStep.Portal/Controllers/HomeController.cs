using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MassTransit.MultiStep.Portal.Models;
using MassTransit.MultiStep.Common.EventMessages;
using MassTransit.MultiStep.CommonBusSetup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Rest;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RestSharp;
using Microsoft.Extensions.Configuration;

namespace MassTransit.MultiStep.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BusManager _busManager;
        private readonly IConfiguration _configuration;
        public HomeController(BusManager busManager, IConfiguration configuration)
        {
            _busManager = busManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SubmitPolicyRequest()
        {
            await _busManager.BusAccess.Publish<IUnderwritingSubmissionSubmitted>(new UnderwritingSubmissionSubmitted()
            {
                SubmissionId = Guid.NewGuid(),
                SubmittedBy = "Ben"
            });

            return View();
        }

        public async Task<IActionResult>About()
        {
            ViewData["Message"] = "Your application description page.";
            ViewData["Token"] = await GetToken();
            return View();
        }

        public async Task<IActionResult> GenerateEmbedToken()
        {
            var token = await GetToken();

            ViewData["Message"] = token;

            return View();
        }

        private async Task<string> GetToken()
        { 
            var client = new RestClient("https://login.microsoftonline.com/common/oauth2/token");
            var request = new RestRequest(Method.POST);
            var requestObj = new
            {
                resource= RestSharp.Extensions.StringExtensions.UrlEncode("https://analysis.windows.net/powerbi/api"),
                client_id = "13469d95-05a7-417c-9ee4-7449e29446ca",
                grant_type = "password",
                username = RestSharp.Extensions.StringExtensions.UrlEncode("ben.kotvis@bluemetal.onmicrosoft.com"),
                password = RestSharp.Extensions.StringExtensions.UrlEncode(_configuration["PowerBI:Password"]),
                scope = "openid"
            };

            var body = $"resource={requestObj.resource}&client_id={requestObj.client_id}&grant_type={requestObj.grant_type}&username={requestObj.username}&password={requestObj.password}&scope={requestObj.scope}";

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            
            var response = await client.ExecuteTaskAsync(request);

            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

            string AadToken = responseObject["access_token"].ToString(),
                reportId = "a91c1b14-ec14-4420-bd90-d19d882885dd",
                groupId = "69d038f6-3fb9-4bb2-99c8-16fd2a9fb184",
                baseUri = "https://api.powerbi.com";
            
            var powerBiApiUrl = $"{baseUri}/v1.0/myorg/groups/{groupId}/reports/{reportId}/GenerateToken";

            var powerBiClient = new RestClient(powerBiApiUrl);

            var powerBiRequest = new RestRequest(Method.POST);
            powerBiRequest.AddHeader("Authorization", $"Bearer {AadToken}");
            powerBiRequest.AddJsonBody(new
            {
                accessLevel = "view"
            });

            var powerBiResponse = await powerBiClient.ExecuteTaskAsync(powerBiRequest);

            var powerBiResponseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(powerBiResponse.Content);

            return powerBiResponseObject["token"].ToString();

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
