# SSRSSubscriptionManager
Import SSRS Subscriptions from an XML file

## Allows fast deployment of multiple SSRS subscriptions
The standard SSRS online wizard is slow and cumbersome. Set an option, click “Next”; set an option, click “Next”. This needs to be done for one subscription at a time. For MLA, it could take up to 30 minutes to create all subscriptions. This might need to be done several times and there is usually some urgency to it. 
SSRS Subscription Manager will allow the instant creation of multiple SSRS Subscriptions, once they have been defined. 

## More Reliable
The standard SSRS wizard is highly prone to human error. Stepping through each of the wizards manually makes it easy to set a setting incorrectly or mix up which subscription you are adding settings to, especially if the operator is distracted in the middle of a wizard.  
SSRS Subscription Manager uses pre-defined XML documents to load the settings. These documents can be tested, verified and then controlled in order to minimise the possibility of the subscriptions being set up incorrectly. 

## Enables controlled documentation of subscription settings
SSRS Reports can be version controlled – but how a subscription is set up might not be documented or controlled. 
The XML documents containing the subscription settings can be added to source control, therefore satisfying audit requirements. 

##	Example XML Document
The following is an example of the XML format for one subscription. You should be able to create subscription settings for most basic subscriptions from this template. 

<?xml version="1.0" encoding="utf-8"?>
<ReportSubscriptions>
    <ReportSubscription ReportName ="Test-Report" 
                        SubscriptionDescription="Some info" 
                        DeliveryMethod="Windows File Share"
                        DataSource="Data Sources/MyDatasource"
                        ItemPath="/Example">
        <EventType>TimedSubscription</EventType>
        <ExtensionSetting>Report Server FileShare</ExtensionSetting>
        <QueryDefinition>
            <CommandText>exec proc_SomeStoredProcedure 'withparameters'</CommandText>
            <CommandType>Text</CommandType>
            <Timeout>30</Timeout>
            <TimeoutSpecified>true</TimeoutSpecified>
        </QueryDefinition>
        <DatasetDefinition>
            <AccentSensitivitySpecified>false</AccentSensitivitySpecified>
            <CaseSensitivitySpecified>false</CaseSensitivitySpecified>
            <KanatypeSensitivitySpecified>false</KanatypeSensitivitySpecified>
            <WidthSensitivitySpecified>false</WidthSensitivitySpecified>
            <!-- Optional
            <Fields>
                <Field>
                    <Name>StartDate</Name>
                    <Alias>StartDateAlias</Alias>
                </Field>
                <Field>
                    <Name>EndDate</Name>
                    <Alias>EndDateAlias</Alias>
                </Field>
            </Fields>-->
        </DatasetDefinition>
        <ExtensionParameters>
            <Parameter Type="ParameterValue">
                <Name>FILENAME</Name>
                <Value>MYFILE.TEMP</Value>
            </Parameter>
            <Parameter Type="ParameterFieldReference">
                <ParameterName>PATH</ParameterName>
                <FieldAlias>FileDir</FieldAlias>
            </Parameter>
            <Parameter Type="ParameterValue">
                <Name>RENDER_FORMAT</Name>
                <Value>CSV</Value>
            </Parameter>
            <Parameter Type="ParameterValue">
                <Name>WRITEMODE</Name>
                <Value>OVERWRITE</Value>
            </Parameter>
            <Parameter Type="ParameterValue">
                <Name>FILEEXTN</Name>
                <Value>FALSE</Value>
            </Parameter>
            <Parameter Type="ParameterValue">
                <Name>USERNAME</Name>
                <Value>test\user1</Value>
            </Parameter>
            <Parameter Type="ParameterValue">
                <Name>PASSWORD</Name>
                <Value>999999</Value>
            </Parameter>                 
        </ExtensionParameters>
        <Parameters>
            <Parameter Type="ParameterFieldReference">
                <ParameterName>StartDate</ParameterName>
                <FieldAlias>StartDate</FieldAlias>
            </Parameter>
            <Parameter Type="ParameterFieldReference">
                <ParameterName>EndDate</ParameterName>
                <FieldAlias>EndDate</FieldAlias>
            </Parameter>
        </Parameters>
        <ScheduleDefinition>
            <StartDateTime>2015-08-11T12:01:00+10:00</StartDateTime>
            <WeeklyRecurrence>
                <WeeksInterval>1</WeeksInterval>
                <DaysOfWeek>
                    <Monday>True</Monday>
                    <Tuesday>True</Tuesday>
                    <Wednesday>True</Wednesday>
                    <Thursday>True</Thursday>
                    <Friday>True</Friday>
                    <Saturday>True</Saturday>
                    <Sunday>True</Sunday>
                </DaysOfWeek>
            </WeeklyRecurrence>
        </ScheduleDefinition>                
    </ReportSubscription>
</ReportSubscriptions>

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
