<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportView.aspx.cs" Inherits="PowerBIEmbedded_AppOwnsData.ReportView"  Debug="true" Async="true" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width" />
    <title>Power BI - Embed Sample</title>

    <script src="https://code.jquery.com/jquery-3.2.1.min.js"
            integrity="sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4="
            crossorigin="anonymous"></script>

    <link rel="stylesheet" type="text/css" href="Content/style/style.css"/>
</head>
<body>
    <form id="form1" runat="server">    
        <div id="errorWrapper" runat="server" visible="false">
        <h2>
            Error
        </h2>
        <pre>
            <label id="lblError" runat="server"></label>
        </pre>
    </div>
        View As:<asp:TextBox ID="txtFilter" Text="FD - 02"  runat="server"></asp:TextBox>
        <asp:Button ID="btnGet" runat="server" Text="Get Report" OnClientClick="openReport();"/>
        <div id="reportContainer"></div>
        <asp:HiddenField ID="hdnAccessToken" runat="server" />
        <asp:HiddenField ID="embedUrl" runat="server" />
        <asp:HiddenField ID="embedReportId" runat="server" />
<script src="https://npmcdn.com/es6-promise@3.2.1"></script>
<script src="scripts/powerbi.js"></script>
        <script type="text/javascript">
            function openReport() {
                var accessToken = document.getElementById("<%= hdnAccessToken.ClientID %>").value;
            var embedUrl = document.getElementById("<%= embedUrl.ClientID %>").value;
            var embedReportId = document.getElementById("<%= embedReportId.ClientID %>").value;
            var filtervalues = document.getElementById("<%= txtFilter.ClientID %>").value;
            const basicFilter = {
                $schema: "http://powerbi.com/product/schema#basic",
                target: {
                    table: "District",
                    column: "District"
                },
                operator: "In",
                values: [filtervalues]
            }

            var models = window['powerbi-client'].models;
            // Embed configuration used to describe the what and how to embed.
            // This object is used when calling powerbi.embed.
            // This also includes settings and options such as filters.
            // You can find more information at https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embed-Configuration-Details.
            var config = {
                type: 'report',
                tokenType: models.TokenType.Embed,
                accessToken: accessToken,
                //filters: ["District eq 'FD - 01'"],
                filters: [basicFilter],
                embedUrl: embedUrl,
                id: embedReportId,
                permissions: models.Permissions.All,
                settings: {
                    filterPaneEnabled: true,
                    navContentPaneEnabled: true
                }
            };

            // Get a reference to the embedded report HTML element
            var reportContainer = $('#reportContainer')[0];


            // Embed the report and display it within the div container.
            var report = powerbi.embed(reportContainer, config);

            }
</script>
    </form>
    
    
</body>
</html>
