using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration; 

namespace Alerting.Function
{
    public static class TriggerAlertLighting
    {
        [FunctionName("TriggerAlertLighting")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Received request!");

            string alert = req.Query["alert"];
            string count = req.Query["count"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            alert = alert ?? data?.alert;
            count = count ?? data?.count;
            
            int number;
            string responseMessage;

            // check if count parameter has been provided or else default to 3
            // Not happy with the nested statements but it works
            if (string.IsNullOrEmpty(count))
            {
                number = 3;
            }
            else
            {
               int tryNumber;
               bool successTryParse;
               // Check if the parameter value for count can be parsed to an int
               // If it doesn't succeed (someone provided a string), default to 3
               successTryParse = int.TryParse(count, out tryNumber);

               if(successTryParse)
                    {
                        number = tryNumber;
                    }
               else
                    {
                        number = 3;
                    }
            }
            
            // Use Configuration Builder to grab required value (Hue URL)
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            string url = config["hueURL"];


            // Create HTTP Client for later use
            HttpClient client = new HttpClient();

            // Create objects for turning the lights on and off
            // Creating an "empty" object to turn off the lights (on is set to false) 
            var propertiesTurnOn = new HueProperties().SetColor(alert);
            var propertiesTurnOff = new HueProperties();  

            responseMessage = $"Light flashed {alert}!";


            // Convert object properties to json
            var jsonTurnOn = JsonConvert.SerializeObject(propertiesTurnOn);
            var jsonTurnOff = JsonConvert.SerializeObject(propertiesTurnOff);

            // Build the body for the API call (PUT) to the Hue bridge
            var dataTurnOn = new StringContent(jsonTurnOn, Encoding.UTF8, "application/json");
            var dataTurnOff = new StringContent(jsonTurnOff, Encoding.UTF8, "application/json");

            // Turn on off for [number] amount of times. Without the delay calls go too fast for the bridge
            for (int i = 0; i < number; i++)
            {
                var response = await client.PutAsync(url, dataTurnOn);
                Task.Delay(2000).Wait();
                var response2 = await client.PutAsync(url, dataTurnOff);
                Task.Delay(2000).Wait();
            }
            // Return response message         
            return new OkObjectResult(responseMessage);
    }
  }
}
