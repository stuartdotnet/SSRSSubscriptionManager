using SSRSSubscriptionManager.SSRSWebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace SSRSSubscriptionManager
{
    public class SubscriptionManager
    {
        private ReportingService2010 client;

        public SubscriptionManager(ReportingService2010 client)
        {
            this.client = client;
        }

        /// <summary>
        /// Loads subscription settings into SubscriptionSettings object
        /// </summary>
        /// <param name="filename">Name of XML file to parse</param>
        /// <returns>Collection of Subscription Settings</returns>
        /// <remarks>This is the meaty bit</remarks>
        public SubscriptionSettings[] LoadSubscriptions(string filename)
        {
            try
            {
                // Load XML Document
                var xml = XElement.Load(filename);

                //// ---------------------------------------------
                //// Read XML into Linq monster
                //// ---------------------------------------------
                var reportSubscriptions = from reportSubscription in xml.Elements("ReportSubscription")
                                          select
                                              new
                                              {
                                                  reportName = (string)reportSubscription.Attribute("ReportName"),
                                                  subscriptionDescription =
                                                    (string)reportSubscription.Attribute("SubscriptionDescription"),
                                                  deliveryMethod =
                                                    (string)reportSubscription.Attribute("DeliveryMethod"),
                                                  dataSource = (string)reportSubscription.Attribute("DataSource"),
                                                  itemPath = (string)reportSubscription.Attribute("ItemPath"),
                                                  eventType = (string)reportSubscription.Element("EventType"),
                                                  queryDefinition = reportSubscription.Element("QueryDefinition"),
                                                  scheduleXml = reportSubscription.Element("ScheduleDefinition"),
                                                  fields =
                                                      from field in reportSubscription.Element("DatasetDefinition").Elements("Fields").Elements()
                                                      select field,
                                                  extentionParameters =
                                                      from extParam in
                                                          reportSubscription.Element("ExtensionParameters").Elements()
                                                      select extParam,
                                                  parameters =
                                                      from param in
                                                          reportSubscription.Element("Parameters").Elements()
                                                      select param,
                                                  extensionSetting = (string)reportSubscription.Element("ExtensionSetting")
                                              };

                var subSettings = new SubscriptionSettings[reportSubscriptions.Count()];   
                var subscriptionIndex = 0;                    

                //// ---------------------------------------------
                //// Run through each subscription and get values
                //// ---------------------------------------------
                foreach (var reportSubscription in reportSubscriptions)
                {
                    subSettings[subscriptionIndex] = new SubscriptionSettings();

                    // Get Subscription Information
                    subSettings[subscriptionIndex].ReportName = reportSubscription.reportName;
                    subSettings[subscriptionIndex].SubscriptionDescription = reportSubscription.subscriptionDescription;
                    subSettings[subscriptionIndex].DeliveryMethod = reportSubscription.deliveryMethod;
                    subSettings[subscriptionIndex].DataSource = reportSubscription.dataSource;
                    subSettings[subscriptionIndex].ItemPath = reportSubscription.itemPath;
                    subSettings[subscriptionIndex].FullReportName = reportSubscription.itemPath + reportSubscription.reportName;

                    // Get EventType setting
                    subSettings[subscriptionIndex].EventType = reportSubscription.eventType;

                    // Get QueryDefinition
                    subSettings[subscriptionIndex].QueryDefinition = new QueryDefinition
                    {
                        CommandText = (string)reportSubscription.queryDefinition.Element("CommandText"),
                        CommandType = (string)reportSubscription.queryDefinition.Element("CommandType"),
                        Timeout = (int)reportSubscription.queryDefinition.Element("Timeout"),
                        TimeoutSpecified = (bool)reportSubscription.queryDefinition.Element("TimeoutSpecified")
                    };

                    // Get Schedule Raw XML
                    subSettings[subscriptionIndex].ScheduleXml = reportSubscription.scheduleXml.ToString();

                    // Get Fields
                    var numberOfFields = reportSubscription.fields.Count();
                    var fieldsList = new Field[numberOfFields];
                    var fieldIndex = 0;

                    foreach (var field in reportSubscription.fields)
                    {
                        fieldsList[fieldIndex] = new Field
                        {
                            Name = (string)field.Element("Name"),
                            Alias = (string)field.Element("Alias")
                        };
                        fieldIndex++;
                    }

                    // Dataset Definition
                    subSettings[subscriptionIndex].DataSetDefinition = new DataSetDefinition
                    {
                        AccentSensitivitySpecified = false,
                        CaseSensitivitySpecified = false,
                        KanatypeSensitivitySpecified = false,
                        WidthSensitivitySpecified = false,
                        Fields = fieldsList
                    };

                    // Get Extension Paramters
                    int numberOfParameterValues = reportSubscription.extentionParameters.Count();
                    var extensionParams = new ParameterValueOrFieldReference[numberOfParameterValues];
                    var extParamIndex = 0;

                    foreach (var extParam in reportSubscription.extentionParameters)
                    {
                        switch ((string)extParam.Attribute("Type"))
                        {
                            case "ParameterValue":
                                var pv = new ParameterValue
                                {
                                    Name = (string)extParam.Element("Name"),
                                    Value = (string)extParam.Element("Value")
                                };
                                extensionParams[extParamIndex] = pv;
                                break;

                            case "ParameterFieldReference":
                                var pfr = new ParameterFieldReference
                                {
                                    ParameterName = (string)extParam.Element("ParameterName"),
                                    FieldAlias = (string)extParam.Element("FieldAlias")
                                };
                                extensionParams[extParamIndex] = pfr;
                                break;
                        }

                        extParamIndex++;
                    }

                    subSettings[subscriptionIndex].ExtensionParameters = extensionParams;

                    // Get (Input) Parameters
                    int numberOfNormalParameterValues = reportSubscription.parameters.Count();
                    var parameters = new ParameterValueOrFieldReference[numberOfNormalParameterValues];
                    var paramIndex = 0;

                    foreach (var param in reportSubscription.parameters)
                    {
                        switch ((string)param.Attribute("Type"))
                        {
                            case "ParameterValue":
                                var pv = new ParameterValue
                                {
                                    Name = (string)param.Element("Name"),
                                    Value = (string)param.Element("Value")
                                };
                                parameters[paramIndex] = pv;
                                break;

                            case "ParameterFieldReference":
                                var pfr = new ParameterFieldReference
                                {
                                    ParameterName = (string)param.Element("ParameterName"),
                                    FieldAlias = (string)param.Element("FieldAlias")
                                };
                                parameters[paramIndex] = pfr;
                                break;
                        }

                        paramIndex++;
                    }

                    subSettings[subscriptionIndex].Parameters = parameters;

                    // Get Extension setting 
                    subSettings[subscriptionIndex].ExtensionSettings = new ExtensionSettings
                    {
                        Extension = reportSubscription.extensionSetting,
                        ParameterValues = extensionParams
                    };

                    // Set up Dataset Definition
                    subSettings[subscriptionIndex].DataSetDefinition.Query = subSettings[subscriptionIndex].QueryDefinition;

                    // Set up Datasource Definition/Reference
                    subSettings[subscriptionIndex].DataSourceReference = new DataSourceReference
                    {
                        Reference = reportSubscription.itemPath + reportSubscription.dataSource
                    };

                    // Prepare Query
                    var preparationResults = new DataSetDefinition();

                    try
                    {
                        bool changed;
                        string[] paramNames;
                        preparationResults =
                            client.PrepareQuery(
                                new DataSource()
                                {
                                    Item = subSettings[subscriptionIndex].DataSourceReference,
                                    Name = "Database" // ?
                                },
                                subSettings[subscriptionIndex].DataSetDefinition,
                                out changed,
                                out paramNames);
                    }
                    catch (SoapException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    subSettings[subscriptionIndex].DataRetrievalPlan = new DataRetrievalPlan
                    {
                        DataSet = preparationResults,
                        Item = subSettings[subscriptionIndex].DataSourceReference
                    };

                    subscriptionIndex++;
                }

                return subSettings;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading subscription settings. Please use absolute path and check XML file is correctly formed.\n\nError Message:\n------------------\n" + e.Message);
            }
        }

        /// <summary>
        /// Creates a new subscription
        /// </summary>
        /// <param name="settings">Settings for the subscription to create</param>
        public void CreateSubscription(SubscriptionSettings settings)
        {
            try
            {
                string subscriptionID = client.CreateDataDrivenSubscription(
                    settings.FullReportName,
                    settings.ExtensionSettings,
                    settings.DataRetrievalPlan,
                    settings.SubscriptionDescription,
                    settings.EventType,
                    settings.ScheduleXml,
                    settings.Parameters);
                Console.WriteLine("\nSubscription ID {0} created.\n", subscriptionID);
                Console.WriteLine("Report name: {0}", settings.ReportName);
                Console.WriteLine("Subscription Description: {0}", settings.SubscriptionDescription);
                Console.WriteLine("Delivery Method: {0}", settings.DeliveryMethod);
                Console.WriteLine("Event Type: {0}", settings.EventType);
                Console.WriteLine("Data Source: {0}", settings.DataSource);
                Console.WriteLine();
            }
            catch (SoapException e)
            {
                Console.WriteLine(e.Detail.InnerText);
                Console.ReadLine();
            }
            catch (NullReferenceException n)
            {
                Console.WriteLine(n.Message);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Displays all subscriptions on the screen
        /// </summary>
        /// <remarks>Not used, maybe useful one day</remarks>
        public string DisplayAllSubscriptions(string path)
        {
            var response = client.ListSubscriptions(path);
            var output = string.Empty;

            foreach (var subscription in response)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("    {0}", subscription.Report);
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Subscription ID: {0}", subscription.SubscriptionID);
                Console.WriteLine("Description: {0}", subscription.Description);
                Console.WriteLine("Status: {0}", subscription.Status);
                Console.WriteLine("Last Executed: {0}", subscription.LastExecuted);
                Console.WriteLine();

                output = subscription.SubscriptionID;
            }
            return output;               
        }
    }
}
