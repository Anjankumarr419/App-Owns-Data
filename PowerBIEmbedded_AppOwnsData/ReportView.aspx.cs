﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using PowerBIEmbedded_AppOwnsData.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PowerBIEmbedded_AppOwnsData
{
    public partial class ReportView : System.Web.UI.Page
    {
        private static readonly string Username = ConfigurationManager.AppSettings["pbiUsername"];
        private static readonly string Password = ConfigurationManager.AppSettings["pbiPassword"];
        private static readonly string AuthorityUrl = ConfigurationManager.AppSettings["authorityUrl"];
        private static readonly string ResourceUrl = ConfigurationManager.AppSettings["resourceUrl"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["clientId"];
        private static readonly string ApiUrl = ConfigurationManager.AppSettings["apiUrl"];
        private static readonly string GroupId = ConfigurationManager.AppSettings["groupId"];
        private static readonly string ReportId = ConfigurationManager.AppSettings["reportId"];
        protected async void Page_Load(object sender, EventArgs e)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            await genarateReportAsync();
            sp.Stop();

        }

        private async Task<string> genarateReportAsync()
        {
            var result = new EmbedConfig();
            try
            {
                result = new EmbedConfig { Username = Username, Roles = null };
                var error = GetWebConfigErrors();
                if (error != null)
                {
                    result.ErrorMessage = error;
                    errorWrapper.Visible = true;
                    lblError.InnerText = error;
                    //return View(result);
                }

                // Create a user password cradentials.
                var credential = new UserPasswordCredential(Username, Password);

                // Authenticate using created credentials
                var authenticationContext = new AuthenticationContext(AuthorityUrl);
                var authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl, ClientId, credential);

                if (authenticationResult == null)
                {
                    result.ErrorMessage = "Authentication Failed.";
                    errorWrapper.Visible = true;
                    lblError.InnerText = result.ErrorMessage;
                }

                var tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");

                // Create a Power BI Client object. It will be used to call Power BI APIs.
                using (var client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials))
                {
                    // Get a list of reports.
                    var reports = await client.Reports.GetReportsInGroupAsync(GroupId);

                    Report report;
                    if (string.IsNullOrEmpty(ReportId))
                    {
                        // Get the first report in the group.
                        report = reports.Value.FirstOrDefault();
                    }
                    else
                    {
                        report = reports.Value.FirstOrDefault(r => r.Id == ReportId);
                    }

                    if (report == null)
                    {
                        result.ErrorMessage = "Group has no reports.";
                        errorWrapper.Visible = true;
                        lblError.InnerText = result.ErrorMessage;
                    }

                    var datasets = await client.Datasets.GetDatasetByIdInGroupAsync(GroupId, report.DatasetId);
                    result.IsEffectiveIdentityRequired = datasets.IsEffectiveIdentityRequired;
                    result.IsEffectiveIdentityRolesRequired = datasets.IsEffectiveIdentityRolesRequired;
                    GenerateTokenRequest generateTokenRequestParameters;
                    // This is how you create embed token with effective identities
                    // Generate Embed Token for reports without effective identities.
                    generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");


                    var tokenResponse = await client.Reports.GenerateTokenInGroupAsync(GroupId, report.Id, generateTokenRequestParameters);

                    if (tokenResponse == null)
                    {
                        result.ErrorMessage = "Failed to generate embed token.";
                        errorWrapper.Visible = true;
                        lblError.InnerText = result.ErrorMessage;
                    }

                    // Generate Embed Configuration.
                    result.EmbedToken = tokenResponse;
                    result.EmbedUrl = report.EmbedUrl;
                    result.Id = report.Id;
                    hdnAccessToken.Value = result.EmbedToken.Token;
                    embedUrl.Value = result.EmbedUrl;
                    embedReportId.Value = result.Id;
                    var str = string.Format("openReport();",
                        result.EmbedToken.Token, result.EmbedUrl, result.Id);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "sas", "openReport();", true);
                    //return View(result);
                }
            }
            catch (HttpOperationException exc)
            {
                result.ErrorMessage = string.Format("Status: {0} ({1})\r\nResponse: {2}\r\nRequestId: {3}", exc.Response.StatusCode, (int)exc.Response.StatusCode, exc.Response.Content, exc.Response.Headers["RequestId"].FirstOrDefault());
                errorWrapper.Visible = true;
                lblError.InnerText = result.ErrorMessage;
            }
            catch (Exception exc)
            {
                result.ErrorMessage = exc.ToString();
                errorWrapper.Visible = true;
                lblError.InnerText = result.ErrorMessage;
            }
        return null;
        }
        private string GetWebConfigErrors()
        {
            // Client Id must have a value.
            if (string.IsNullOrEmpty(ClientId))
            {
                return "ClientId is empty. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }

            // Client Id must be a Guid object.
            Guid result;
            if (!Guid.TryParse(ClientId, out result))
            {
                return "ClientId must be a Guid object. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }

            // Group Id must have a value.
            if (string.IsNullOrEmpty(GroupId))
            {
                return "GroupId is empty. Please select a group you own and fill its Id in web.config";
            }

            // Group Id must be a Guid object.
            if (!Guid.TryParse(GroupId, out result))
            {
                return "GroupId must be a Guid object. Please select a group you own and fill its Id in web.config";
            }

            // Username must have a value.
            if (string.IsNullOrEmpty(Username))
            {
                return "Username is empty. Please fill Power BI username in web.config";
            }

            // Password must have a value.
            if (string.IsNullOrEmpty(Password))
            {
                return "Password is empty. Please fill password of Power BI username in web.config";
            }

            return null;
        }
    }
}