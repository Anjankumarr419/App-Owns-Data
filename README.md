# App-Owns-Data - Working sample code with Both Asp.Net and MVC 

-Included Filtering report before it presents to the user.

Read this documentation to prepare your environment
https://docs.microsoft.com/en-us/power-bi/developer/embedding-content

To see embedded report, dashboard and tile, you need to fill missing details in web.config.

clientId - Id of AAD application registered as NATIVE app. 

groupId - group or workspace Id in Power BI which contains reports, dashboards and tiles you want to embed.

pbiUsername - Power BI username (e.g. Email). Must be an admin of the group above.

pbiPassword - password of Power BI user above.


### ASP.Net
ReportView.aspx -  you can view Power Bi report using Asp.net. 
Here Filter report based on the perticular distict is aslo included. you can add your own filters based on your requirement. 

### Important

For security reasons, in real application, don't save the user and password in web.config. Consider using KeyVault

Registration Example:

![regexample](https://cloud.githubusercontent.com/assets/23071967/23340723/fc032efe-fc43-11e6-9a8f-13e40cb32d97.png)

## Step 1 - Change Web.config
Copy Client Id and Client secret to web.config file

![regexample](https://cloud.githubusercontent.com/assets/23071967/23340740/48d4f640-fc44-11e6-8f31-dd273d26a61e.png)

## Troubleshooting

### Visual Studio 2013
To resolve a 'CS0012:Predefined type 'System.Object' is not defined or imported' error, please update web.config.

Find line:
 ```xml
 <compilation debug="true" targetFramework="4.5"/>
 ```
 
 And modify it to:
 
 ```xml
 <compilation debug="true" targetFramework="4.5">
  <assemblies>     
    <add assembly="System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />   
  </assemblies>
</compilation>
