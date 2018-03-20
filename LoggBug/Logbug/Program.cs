using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logbug.Library;
//using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Preview.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Logbug
{
    public class Program
    {
        //To create a personal tokan please see the URL  Authenticating with personal access tokens
        //https://docs.microsoft.com/en-us/vsts/integrate/get-started/authentication/pats

        //For TFS, instance is {server:port}/tfs and by default the port is 8080. The default collection is DefaultCollection, but can be any collection.

        public static void Main(string[] args)
        {
            string connectionUrl = null, token = null, project = null;
            args = new string[] { "/url:kanchansharmaqa.visualstudio.com", "/token:vmftoxdnevias52hvenyinysppsxff5kneqemv32o7vkryahmhva", "/project:Training" };

            if (args.Length == 0)
            {
                Console.WriteLine("Runs the sample on a Team Services account or Team Foundation Server instance to create a new bug.");                
                Console.WriteLine("");
            }

            try
            {
                CheckArguments(args, out connectionUrl, out token, out project);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

            CreateBug cbug = new CreateBug(connectionUrl, token, project);

            Console.WriteLine("Executing quick start sample to create a Bug...");
            Console.WriteLine("");

            //todo: Create a new bug using HttpClient.
            //string BugTitle = null; string ReproSteps = null; int Priority= 3; string Area= null; string Iteration=null;

            string BugTitle = "Demo bug title Neelam"; string ReproSteps = "This is a demo 1 bug description"; int Priority = 3;
            string Area = @"Training\WebFramework"; string Iteration = @"Training\Release 1"; 
            string Severity = cbug.SeverityValue(CreateBug.Severity.High);

            WorkItem wrkItemCreated = cbug.CreateBugUsingDotNetClientLib(BugTitle, ReproSteps, Priority, Area, Iteration, Severity);

            //string response = cbug.CreateBugHttpClient(BugTitle, ReproSteps, Priority, Area, Iteration, Severity);
            //todo: Create a new bug using HttpClient.
        }
        private static void ShowUsage()
        {
            Console.WriteLine("Runs the Quick Start samples on a Team Services account or Team Foundation Server instance.");
            Console.WriteLine("");
            Console.WriteLine("These samples are to provide you the building blocks of using the REST API's in Work Item Tracking.");
            Console.WriteLine("Examples are written using the .NET client library and using direct HTTP calls. We recommend, that");
            Console.WriteLine("whenever possible, you use the .NET client library.");
            Console.WriteLine("");
            Console.WriteLine("!!WARNING!! Some samples are destructive. Always run on a test account or collection.");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /url:kanchansharmaqa.visualstudio.com /token:personalaccesstoken /project:projectname");
            Console.WriteLine("");

            Console.ReadKey();
        }
        private static void CheckArguments(string[] args, out string connectionUrl, out string token, out string project)
        {
            connectionUrl = null;
            token = null;
            project = null;

            Dictionary<string, string> argsMap = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                if (arg[0] == '/' && arg.IndexOf(':') > 1)
                {
                    string key = arg.Substring(1, arg.IndexOf(':') - 1);
                    string value = arg.Substring(arg.IndexOf(':') + 1);

                    switch (key)
                    {
                        case "url":
                            connectionUrl = value;
                            break;

                        case "token":
                            token = value;
                            break;

                        case "project":
                            project = value;
                            break;
                        default:
                            throw new ArgumentException("Unknown argument", key);
                    }
                }
            }

            if (connectionUrl == null || token == null)
                throw new ArgumentException("Missing required arguments");

        }
    }
}
