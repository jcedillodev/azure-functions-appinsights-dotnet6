# azure-functions-appinsights-dotnet6
This project is a sample .NET 6 function app which defines an Http trigger function that integrates Application Insights to log telemetry on function executions with the Azure Monitoring service.

# <h1>Repro Steps</h1>

The project was setup following the below documentation.

<ul>
  <li>
    <a href="https://docs.microsoft.com/en-us/azure/azure-functions/functions-monitoring" target="_blank">Monitor executions in Azure Functions</a>
  </li>
  <li>
    <a href="https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v2%2Ccmd#log-custom-telemetry-in-c-functions" target="_blank">Develop C# class library functions using Azure Functions - Log Custom Telemetry</a>
  </li>
  <li>
    <a href="https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=in-process%2Cfunctionsv2&pivots=programming-language-csharp" target="_blank">Azure Functions HTTP Trigger</a>
  </li>
</ul>

NOTE: Project setup using VS 2022 installed with Azure specific workloads - see documentation below:
<a href="https://docs.microsoft.com/en-us/dotnet/azure/configure-visual-studio" target="_blank">Configure Visual Studio for Azure development with .NET</a>
<a href="https://docs.microsoft.com/en-us/visualstudio/azure/overview-azure-integration?view=vs-2022" target="_blank">Overview: Azure integration</a>

Dependencies:
<ol>
  <li>
    <a href="https://www.nuget.org/packages/Microsoft.Azure.WebJobs" target="_blank">Microsoft.Azure.WebJobs (3.0.33)</a>
  </li>
  <li>
    <a href="https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Logging.ApplicationInsights" target="_blank">Microsoft.Azure.WebJobs.Logging.ApplicationInsights (3.0.33)</a>
  </li>
</ol>

NOTE: 
