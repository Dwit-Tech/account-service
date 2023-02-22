using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace DwitTech.AccountService.WebApi.Controllers
{
    public class ExternalEmailController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            var url = "https://reqres.in/api/user";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                name = "ActivationEmail",
                job = "EndpointApi"
            };
            var bodyy = JsonConvert.SerializeObject(body);
            request.AddBody(bodyy, "application/json");
            RestResponse response= await client.ExecuteAsync(request);
            var output = response.Content;
            return View();
        }

    }
}
