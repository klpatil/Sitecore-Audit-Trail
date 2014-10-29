Sitecore Audit Trail
====================

Sitecore Audit Trail (SCAuditTrail) allows users to see Sitecore Audit Trail -- Who did What changes and When?

![Sitecore Audit Trail](https://sitecorebasics.files.wordpress.com/2014/10/scaudittrail-v1.gif "Sitecore Audit Trail")

##Main Features

1. Answers following questions:
 1. Who did change on particular item?
 2. When it happened?
 3. What all actions happened on particular day?
2. Does all tracking in back end - No effect on performance! [Uses power of log4net Appenders]
3. Shows actions at Item Level and Site Level
4. You can do search, pagination, and sorting!
5. Fully Configurable
6. No code changes required to do audit tracking
7. Security has been applied -- User needs to be logged in to access it.
8. Accessible from Context Menu of an Item And Review Tab

##How to Download, Install and Configure?

> Just a note : Please don't get bored while doing configuration steps. They are Just one time steps!

1. Download Sitecore Package from Packages\V1 folder Install it using Sitecore Installation Wizard.
2. Database related changes:
 1. We will need a separate Database to store all Audit Entries. To do that download and run SQL\Create-Database-v1.sql script. [Before running if you need to do any changes to script, please do it. Please avoid changing Database Name. And because of some reason you need to that. Then you need to download source and do changes at DataAccessLevel. As we are using LinqToSQL]
 2. Now, Database is created. We need to have a table to store data. To do that download and run SQL\Create-Table-v1.sql script it will create Logs Table with required columns.

3. Do following configurations [If Copying from Web.Config opened in Visual Studio makes your life easier, Please feel free to Download Web.Config from here and copy following details from that file directly] :
 1. Add SCAuditTrail DB entry in ConnectionStrings.Config:
 
``` config
<add name="SCAuditTrailConnectionString"
           connectionString="Data Source=<YOURSQLINSTANCENAME>;Initial Catalog=SCAuditTrail;User ID=<SQLUSERNAME>;Password=<SQLPASSWORD>" providerName="System.Data.SqlClient" />
``` 

**Now, Change connectionString attribute's value as per your environment.**

 2. Add Appender Settings : To do that Open Web.Config file and find `<appender name="PublishingLogFileAppender"` add below configurations once **PublishingLogFileAppender** appender ends i.e. `</appender>` :


``` config
  <!--type="log4net.Appender.ADONetAppender, Sitecore.Logging" >-->
    <appender name="SCBasicsAuditTrailDBAppender" type="SCBasics.AuditTrail.Appender.SitecoreDatabaseLogAppender,
              SCBasics.AuditTrail" >
      <!-- Filter items where message field start with 'AUDIT....' -->
      <filter type="log4net.Filter.StringMatchFilter">
        <regexToMatch value="^AUDIT" />
      </filter>

      <!-- Deny all other items -->
      <filter type="log4net.Filter.DenyAllFilter" />
      <!--his is because logs are buffered until a given buffersize is reached.
      The default value is 512, but you can lower this by modifying the following line of code-->
      <bufferSize value="1" />
      <param name="ConnectionType" value="System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, 

Culture=neutral, PublicKeyToken=b77a5c561934e089" />
      <!--Double SLASH is Very Important-->
      <param name="ConnectionString" value="user id=<SQLUSERNAME>;password=<SQLPASSWORD>;Data 

Source=<SQLINSTANCENAME>;Database=SCAuditTrail" />
      <!--<param name="CommandText" value="INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message]) VALUES 

(@log_date, @thread, @log_level, @logger, @message)" />-->
      <param name="CommandText" value="INSERT INTO Logs ([Date],[Thread],[Level],[Logger],[Message],[Exception], 

[SCUser], [SCAction], [SCItemPath], [SCLanguage] , [SCVersion] , [SCItemId], [SiteName],[SCMisc]) VALUES (@log_date, 

@thread, @log_level, @logger, @message, @exception, @scuser,  @scaction, @scitempath, @sclanguage, @scversion, 

@scitemid, @sitename, @scmisc)" />
      <param name="Parameter">
        <param name="ParameterName" value="@log_date" />
        <param name="DbType" value="DateTime" />
        <param name="Layout" type="log4net.Layout.PatternLayout">
          <param name="ConversionPattern" value="%d{yyyy&apos;-&apos;MM&apos;-&apos;dd 

HH&apos;:&apos;mm&apos;:&apos;ss&apos;.&apos;fff}" />
        </param>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@thread" />
        <param name="DbType" value="String" />
        <param name="Size" value="255" />
        <param name="Layout" type="log4net.Layout.PatternLayout">
          <param name="ConversionPattern" value="%t" />
        </param>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@log_level" />
        <param name="DbType" value="String" />
        <param name="Size" value="50" />
        <param name="Layout" type="log4net.Layout.PatternLayout">
          <param name="ConversionPattern" value="%p" />
        </param>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@logger" />
        <param name="DbType" value="String" />
        <param name="Size" value="255" />
        <param name="Layout" type="log4net.Layout.PatternLayout">
          <param name="ConversionPattern" value="%c" />
        </param>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@message" />
        <param name="DbType" value="String" />
        <param name="Size" value="4000" />
        <param name="Layout" type="log4net.Layout.PatternLayout">
          <param name="ConversionPattern" value="%m" />
        </param>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@exception" />
        <dbType value="String" />
        <size value="2000" />
        <layout type="log4net.Layout.ExceptionLayout" />
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@scuser" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="scuser" />
        </layout>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@scaction" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="scaction" />
        </layout>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@scitempath" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="scitempath" />
        </layout>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@sclanguage" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="sclanguage" />
        </layout>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@scversion" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="scversion" />
        </layout>
      </param>      
      <param name="Parameter">
        <param name="ParameterName" value="@scitemid" />
        <dbType value="String" />
        <size value="38" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="scitemid" />
        </layout>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@sitename" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="sitename" />
        </layout>
      </param>
      <param name="Parameter">
        <param name="ParameterName" value="@scmisc" />
        <dbType value="String" />
        <size value="255" />
        <layout type="SCBasics.AuditTrail.Log4NetLayout.PropertyLayout,SCBasics.AuditTrail">
          <param name="PropertyName" value="scmisc" />
        </layout>
      </param>
    </appender>
    
``` 
**Now, Change connectionString attribute's value as per your environment. While doing so PLEASE REMEMBER WHILE ADDING Data Source Value use Double slash instead of Single Slash. i.e. Instead of (local)\SQE2K8R2 use (local)\\\SQE2K8R2 ELSE IT WON'T WORK**

**Also, Default bufferSize value is 512. But we recommend having it 1 during initial configuration. Else you've to wait for a while to see affect of your changes. Once all good. Then would suggest changing this value as per your need.**

 3. Add Appender Reference. We are getting closed. You've added Appender. Now, as you know we need to add this Appender in root for that find `<appender-ref ref="LogFileAppender"/>` in **Web.Config** and add following tag after that tag:
 
``` config
<appender-ref ref="SCBasicsAuditTrailDBAppender" /> 
```
So, It should look like following: 

``` config
<root>
      <priority value="DEBUG"/>
      <appender-ref ref="LogFileAppender"/>
      <appender-ref ref="SCBasicsAuditTrailDBAppender" />
    </root>
```

**Go ahead and start enjoying SCAuditTrail!**

##Troubleshooting
If it's not working for you. Then would recommend verify steps given above. [Better to get other pair of eyes!] And still if all looks good then do add following configuration in your Web.Config file before `</configuration>` Pleas make sure you modify **<DATAFOLDERPATH>**
token as per your environment. 

``` config
<system.diagnostics>
    <trace autoflush="true" indentsize="4">
      <listeners>
        <add name="myListener" type="System.Diagnostics.TextWriterTraceListener"
             initializeData="<DATAFOLDERPATH>\error.log" />
        <remove name="Default" />
      </listeners>
    </trace>
  </system.diagnostics>
```
Request Site again and check error.log. It will reveal the **REAL** Error.

##How It Works?
Basically this module is inspired from following links:

 1. [http://sdn.sitecore.net/scrapbook/how%20to%20make%20sitecore%206%20write%20audit%20log%20to%20its%20own%20file.aspx](http://sdn.sitecore.net/scrapbook/how%20to%20make%20sitecore%206%20write%20audit%20log%20to%20its%20own%20file.aspx)
 2. [http://sitecoreblog.alexshyba.com/2010/07/sitecore-logging-write-it-to-sql.html](http://sitecoreblog.alexshyba.com/2010/07/sitecore-logging-write-it-to-sql.html)

It uses power of Log4Net's ADO.Net Appender. As you know Sitecore uses log4Net for logging. So, using Log4NetAppender recording only AUDIT records in Database. And then using Sitecore command integrated it with CE. Simple!

##Road Map

**This is one of my dream module** I've uploaded 4-5 modules on Sitecore MarketPlace. But I think this one beats all my earlier modules [_Life is all about breaking your own records_]. While working on it. Had lot of ideas. Most of them implemented and then at one point had to stop. Because Best is the enemy of Good and I learn during my experience. it's good to follow ***YAGNI [You Aren't Gonna Need It]*** Principle : [http://www.hanselman.com
/blog/analysisparalysisoverthinkingandknowingtoomuchtojustcode.aspx](http://www.hanselman.com%20/blog/analysisparalysisoverthinkingandknowingtoomuchtojustcode.aspx)

> But I've listed down all my ideas and open for ideas from here. As we live in the democratic world. Would like to know from you (Yes, You!) Which feature you want first to be implemented or added. You can do that from here : [http://tinyurl.com/SCAuditTrailIdeas](http://tinyurl.com/SCAuditTrailIdeas)

And would be happy, rather than me working on all your ideas, You would like to do a pull request and start working on your idea. Then I must say it's a Nobel !dea! Just do it!

## Known issues
1. If you open a publish dialog and close it, without publishing. Then also it does an entry of Publish Item.
2. Restore Item entry is not getting caught as of now.

**But for this you can add your own custom event hooks subscribing for the events which are not working fine. And making sure you do - Audit Trail entries as per your need then rest all should work automatically!**

>Found any bug? Got suggestion/feedback/comment, Share it here!


