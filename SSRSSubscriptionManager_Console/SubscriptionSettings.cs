namespace SSRSSubscriptionManager
{
    using SSRSSubscriptionManager.SSRSWebReference;

    /// <summary>
    /// Class to hold subscription settings
    /// </summary>
    public class SubscriptionSettings
    {
        public string ReportName { get; set; }

        public string FullReportName { get; set; }

        public string SubscriptionDescription { get; set; }

        public string DeliveryMethod { get; set; }

        public string DataSource { get; set; }

        public string ItemPath { get; set; }

        // Query Definition
        public QueryDefinition QueryDefinition { get; set; }

        public string EventType { get; set; }

        public string ScheduleXml { get; set; }

        // Dataset Definition
        public DataSetDefinition DataSetDefinition { get; set; }

        // Datasource Definition - Pay attention to name
        public DataSourceReference DataSourceReference { get; set; }

        // Extension Settings
        public ExtensionSettings ExtensionSettings { get; set; }

        // DataRetrievalPlan
        public DataRetrievalPlan DataRetrievalPlan { get; set; }

        // Extension Parameters
        public ParameterValueOrFieldReference[] ExtensionParameters;   
     
        // Parameters
        public ParameterValueOrFieldReference[] Parameters;  
    }
}
