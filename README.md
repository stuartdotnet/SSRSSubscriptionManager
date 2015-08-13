# SSRSSubscriptionManager
Import SSRS Subscriptions from an XML file

## Allows fast deployment of multiple SSRS subscriptions
The standard SSRS online wizard is slow and cumbersome. Set an option, click “Next”; set an option, click “Next”. This needs to be done for one subscription at a time. For large sets of subscriptions, it could take up to 30 minutes to create all subscriptions. This might need to be done several times and there may be urgency to it. 

SSRS Subscription Manager will allow the instant creation of multiple SSRS Subscriptions, once they have been defined. 

## More Reliable
The standard SSRS wizard is highly prone to human error. Stepping through each of the wizards manually makes it easy to set a setting incorrectly or mix up which subscription you are adding settings to, especially if the operator is distracted in the middle of a wizard.  

SSRS Subscription Manager uses pre-defined XML documents to load the settings. These documents can be tested, verified and then controlled in order to minimise the possibility of the subscriptions being set up incorrectly. 

## Enables controlled documentation of subscription settings
SSRS Reports can be version controlled – but how a subscription is set up might not be documented or controlled. 
The XML documents containing the subscription settings can be added to source control, therefore satisfying audit requirements. 

##	Example XML Document
See example.xml for an example of the XML format for one subscription. You should be able to create subscription settings for most basic subscriptions from this template. 

## XML Document Specification
###	ReportSubscriptions
Root element, contains one or multiple <ReportSubscription> blocks
###	ReportSubscription
Defines all the elements of a single subscription. 
####	ReportName (Attribute) 
The Report name to which the subscription will apply. This needs to be an existing report and the text must match its name exactly.
####	SubscriptionDescription (Attribute)
Contains a friendly description of the subscription

####	DeliveryMethod (Attribute)
Must contain one of the following phrases exactly:
•	Windows File Share
•	Email
This specifies whether the report will be an email subscription or a data batch file drop. 
####	DataSource (Attribute)
This contains the Server Path of the data source. Note this does NOT start with a slash (/).
####	ItemPath (Attribute)
This contains the Server Path of the report. Note this DOES start with a slash (/).
###	EventType
###	ExtensionSetting
###	QueryDefinition
Contains parameters from “Step 3” of the Wizard; Command, Timeout etc. 
####	Command Text
Contains a SQL command or query that returns a list of recipients and optionally returns fields used to vary delivery settings and report parameter values for each recipient.

## References
In creating this application the following links were very useful, and may be useful for future development: 

•	Programmatically Create Data Driven Subscriptions in SQL Server 2005/2008
http://www.sqlservercurry.com/2009/07/programmatically-create-data-driven.html 

•	Programmatically Playing With SSRS Subscriptions
http://www.codeproject.com/Articles/36009/Programmatically-Playing-With-SSRS-Subscriptions 

•	Create a Data-Driven Subscription (SSRS Tutorial)
http://msdn.microsoft.com/en-us/library/ms169673.aspx 
