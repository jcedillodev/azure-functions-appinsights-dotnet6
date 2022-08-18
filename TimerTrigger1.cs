using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace dotnetfctappappinsightsint
{
    public class TimerTrigger1
    {
        private readonly TelemetryClient telemetryClient;

        // Using dependency injection will guarantee that you use the same configuration for telemetry collected automatically and manually.
        public TimerTrigger1(TelemetryConfiguration telemetryConfiguration)
        {
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        // Timer Trigger Example
        [FunctionName("TimerTrigger1")]
        public void Run([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {// Function will fire at the top of every hour daily - for more info, review NCRONTAB expressions: https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer?tabs=in-process&pivots=programming-language-csharp#ncrontab-expressions

            string timerFctName = "TimerTrigger1";
            log.LogInformation("C# timer trigger function has started executing.");
            DateTime start = DateTime.UtcNow;

            try
            {
                // Custom message for EventTelemetry object
                string eventTelementryMsg = $"Function {timerFctName} has executed.";

                // Write an event to the customEvents table.
                var evt = new EventTelemetry(eventTelementryMsg);

                // Track EventTelemetry object
                this.telemetryClient.TrackEvent(evt);

                // Log a custom dependency in the dependencies table using site (johncedillo.com) health check endpoint
                var dependency = new DependencyTelemetry
                {
                    Name = "GET api/ping",
                    Target = "jcrgutildotnetfctapp.azurewebsites.net",
                    Data = "jcrgutildotnetfctapp.azurewebsites.net/api/ping",
                    Timestamp = start,
                    Duration = DateTime.UtcNow - start,
                    Success = true
                };

                // Track custom dependency using telemetry client
                this.telemetryClient.TrackDependency(dependency);

                // Generate a custom metric, in this case let's use function duration - converts System.TimeSpan (double) to int data type
                this.telemetryClient.GetMetric("functionExecutionTime").TrackValue((int)(DateTime.UtcNow - start).TotalMinutes);
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
            }
            log.LogInformation("C# timer trigger function has finished executing.");
        }
    }
}
