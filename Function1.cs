
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AdaptiveCards;
using Newtonsoft.Json.Linq;

namespace ChatPO1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.text;


            //This is the response the bot will send back to Teams - in this case, it will place any text it receives in the 'name' string, including its own '@teamsbot'
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //-------------------------------Teams Formatting -----------------------------
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0));

            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = responseMessage,
                Size = AdaptiveTextSize.Default,
                Wrap = true
            });

            var jsonMessage = @"{
            type:'message',
            attachments:[
            {
                contentType:'application/vnd.microsoft.card.adaptive',
                contentUrl:'null',
                content:" +
                    card.ToJson() +
            @"}]
            }";

            JObject jsonObj = JObject.Parse(jsonMessage);
            //------------------------------------------------------------------------------

            return (ActionResult)new OkObjectResult(jsonObj);
        }
    }
}
