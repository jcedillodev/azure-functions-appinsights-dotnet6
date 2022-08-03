using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace dotnetfctappappinsightsint
{
    public class Functions
    {
        private readonly TelemetryClient telemetryClient;

        // Using dependency injection will guarantee that you use the same configuration for telemetry collected automatically and manually.
        public Functions(TelemetryConfiguration telemetryConfiguration)
        {
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        // New Function codebase - reference documentation:
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v2%2Ccmd#log-custom-telemetry-in-c-functions
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=in-process%2Cfunctionsv2&pivots=programming-language-csharp
        
        [FunctionName("HttpTrigger1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req, ExecutionContext context, ILogger log)
        {
            string fctName = "HttpTrigger1";
            log.LogInformation("C# HTTP trigger function processed a request.");
            DateTime start = DateTime.UtcNow;

            try
            {
                // Parse query parameter
                string name = req.Query
                    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                    .Value;

                // Retrieve request body 'name' key value if a JSON body is sent on POST calls                
                string reqName = req.Query["name"];

                // Request request body passed on POST calls with a JSON object/key of 'name' to retrieve it's value
                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                reqName = reqName ?? data?.name;

                // Set message passed when registering custom app insights telemetry event if name is defined in POST call with a JSON request body
                // If not, set default message
                string eventTelementryMsg = string.IsNullOrEmpty(reqName)
                    ? $"Function {fctName} was called."
                    : $"Function {fctName} called by {reqName}.";

                // Write an event to the customEvents table.
                var evt = new EventTelemetry(eventTelementryMsg);

                evt.Context.User.Id = name;
                this.telemetryClient.TrackEvent(evt);

                // Generate a custom metric, in this case let's use ContentLength.
                this.telemetryClient.GetMetric("contentLength").TrackValue(req.ContentLength);

                // Log a custom dependency in the dependencies table.
                var dependency = new DependencyTelemetry
                {
                    Name = "GET api/planets/1/",
                    Target = "swapi.co",
                    Data = "https://swapi.co/api/planets/1/",
                    Timestamp = start,
                    Duration = DateTime.UtcNow - start,
                    Success = true
                };
                dependency.Context.User.Id = name;
                this.telemetryClient.TrackDependency(dependency);

                return (ActionResult)new OkObjectResult(eventTelementryMsg);
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                return new BadRequestObjectResult($"Error: {ex}");
            }
        }
    }
}
