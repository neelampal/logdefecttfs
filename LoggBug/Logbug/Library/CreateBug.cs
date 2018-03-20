using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System.Collections.Specialized;


namespace Logbug.Library
{
    class CreateBug
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        public CreateBug()
        {
            _uri = "https://accountname.visualstudio.com";
            _personalAccessToken = "personal access token";
            _project = "project name";
        }

        //Credentials
        public CreateBug(string url, string pat, string project)
        {
            _uri = url;
            _personalAccessToken = pat;
            _project = project;
        }
        public enum Severity { Critical = 0, High, Medium, Low }

        /// <summary>
        /// Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>    
        public WorkItem CreateBugUsingDotNetClientLib(string BugTitle, string ReproSteps, int Priority, string Area, string Iteration, string Severity)
        {
            WorkItem result = new WorkItem();
            try
            {
                string url = "https://" + _uri+"/";
                Uri uri = new Uri(url);
                string personalAccessToken = _personalAccessToken;
                string project = _project;

                VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);
                JsonPatchDocument patchDocument = new JsonPatchDocument();

                #region :Add fields and their values to your patch document
                //add fields and their values to your patch document

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Title",
                        Value = BugTitle
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                        Value = ReproSteps
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.Common.Priority",
                        Value = Priority.ToString()
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.Common.Severity",
                        Value = Severity
                    }
                );
                #endregion

                VssConnection connection = new VssConnection(uri, credentials);
                WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

                try
                {
                    result = workItemTrackingHttpClient.UpdateWorkItemTemplateAsync(patchDocument, project, "Bug").Result;                    
                    Console.WriteLine("Bug Successfully Created: Bug #{0}", result.Id);
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine("Error creating bug: {0}", ex.InnerException.Message);
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public string CreateBugHttpClient(string BugTitle, string ReproSteps, int Priority, string Area, string Iteration, string severity)
        {
            string result = null;
            try
            {
                string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _personalAccessToken)));

                #region :Define Bug fields                
                Object[] patchDocument = new Object[4];

                if (!string.IsNullOrEmpty(BugTitle))
                    patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = BugTitle };
                if (!string.IsNullOrEmpty(ReproSteps))
                    patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.TCM.ReproSteps", value = ReproSteps };
                if (!string.IsNullOrEmpty(Priority.ToString()))
                    patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = Priority.ToString() };
                if (!string.IsNullOrEmpty(severity))
                    patchDocument[3] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = severity };
                #endregion
                
                //use the Http Client
                using (HttpClient client = new HttpClient())
                {
                    //set our headers and authorization
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    //serialize the fields array into a json string
                    var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json");

                    HttpMethod method = new HttpMethod("PATCH");
                    
                    HttpRequestMessage request = new HttpRequestMessage(method, "https://" + _uri + "/" + _project + "/_apis/wit/workitems/$Bug?api-version=2.2") { Content = patchValue };
                    var response = client.SendAsync(request).Result;

                    //if the response is successfull, set the result to the workitem object
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result.ToString();
                    }
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public string SeverityValue(Severity severity)
        {
            string severityValue = null;
            try
            {
                #region :Bug Severity Value                
                switch (severity)
                {
                    case Severity.Critical:
                        severityValue = "1 - Critical";
                        break;
                    case Severity.High:
                        severityValue = "2 - High";
                        break;
                    case Severity.Medium:
                        severityValue = "3 - Medium";
                        break;
                    case Severity.Low:
                        severityValue = "4 - Low";
                        break;
                    default:
                        break;
                }
                #endregion
            }
            catch (Exception e)
            {
            }
            return severityValue;

        }
    }
}