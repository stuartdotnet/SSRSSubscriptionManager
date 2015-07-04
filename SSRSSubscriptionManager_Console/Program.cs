namespace SSRSSubscriptionManager
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Web.Services.Protocols;
    using System.Xml.Linq;

    using SSRSSubscriptionManager.SSRSWebReference;

    /// <summary>
    /// Program to create and manage SSRS Subscriptions
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets connection to Reporting web service
        /// </summary>
        public static ReportingService2010 Client
        {
            get
            {
                try
                {
                    var url = ConfigurationManager.AppSettings["Server"];
                    var client = new ReportingService2010();
                    var uri = new Uri(url);
                    var credentials = CredentialCache.DefaultCredentials;
                    var credential = credentials.GetCredential(uri, "Basic");
                    client.Credentials = credential;

                    return client;
                }
                catch (ConfigurationErrorsException e)
                {
                    Console.WriteLine("{0}", e);
                }

                return null;
            }
        }

        /// <summary>
        /// Creates SSRS subscriptions based on settings stored in XML file.
        /// </summary>
        /// <param name="args">Command line arguements: filename of settings file</param>
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SSRSSubscriptionManager filename reportpath");
                Console.ReadLine();
                System.Environment.Exit(0);
            }

            var settingsFile = args[0];
            string path = string.IsNullOrEmpty(args[1]) ? string.Empty : args[1];

            var sm = new SubscriptionManager(Client);  
            sm.DisplayAllSubscriptions(path);

            Console.WriteLine("\n\nCreating subscriptions from {0} \non {1} \n\nPress Enter to Continue...\n", settingsFile, Client.Url);
            Console.ReadLine();
            Console.WriteLine("\n\nPlease Wait...\n");

            var settings = sm.LoadSubscriptions(settingsFile);

            foreach (var subscriptionSettings in settings)
            {
                sm.CreateSubscription(subscriptionSettings);
            }

            Console.WriteLine("All done. Please come again!");
            Console.ReadLine();
        }             
    }
}
