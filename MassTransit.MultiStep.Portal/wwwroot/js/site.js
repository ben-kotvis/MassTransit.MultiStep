// Write your JavaScript code.

$("#runReport").click(function () {
    // Read embed application token from textbox
    var txtAccessToken = $("#tokenValue").val();

    // Read embed URL from textbox
    var txtEmbedUrl = "https://app.powerbi.com/reportEmbed?reportId=a91c1b14-ec14-4420-bd90-d19d882885dd&groupId=69d038f6-3fb9-4bb2-99c8-16fd2a9fb184";

    // Read report Id from textbox
    var txtEmbedReportId = "a91c1b14-ec14-4420-bd90-d19d882885dd";



    // Get models. models contains enums that can be used.
    var models = window['powerbi-client'].models;

    // We give All permissions to demonstrate switching between View and Edit mode and saving report.
    var permissions = models.Permissions.All;

    // Embed configuration used to describe the what and how to embed.
    // This object is used when calling powerbi.embed.
    // This also includes settings and options such as filters.
    // You can find more information at https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embed-Configuration-Details.
    var config = {
        type: 'report',
        //tokenType: tokenType == '0' ? models.TokenType.Aad : models.TokenType.Embed,
        tokenType: models.TokenType.Embed,
        accessToken: txtAccessToken,
        embedUrl: txtEmbedUrl,
        id: txtEmbedReportId,
        permissions: permissions,
        settings: {
            filterPaneEnabled: true,
            navContentPaneEnabled: true
        }
    };

    // Get a reference to the embedded report HTML element
    var embedContainer = $('#embedContainer')[0];

    // Embed the report and display it within the div container.
    var report = powerbi.embed(embedContainer, config);

    // Report.off removes a given event handler if it exists.
    report.off("loaded");

    // Report.on will add an event handler which prints to Log window.
    report.on("loaded", function () {
        Log.logText("Loaded");
    });

    report.on("error", function (event) {
        Log.log(event.detail);

        report.off("error");
    });

    report.off("saved");
    report.on("saved", function (event) {
        Log.log(event.detail);
        if (event.detail.saveAs) {
            Log.logText('In order to interact with the new report, create a new token and load the new report');
        }
    });
});
