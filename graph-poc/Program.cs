// https://learn.microsoft.com/en-us/azure/app-service/scenario-secure-app-access-microsoft-graph-as-app?tabs=azure-cli

using System.Net.Http.Headers;
using Azure.Identity;
using graph_poc;
using Microsoft.Graph;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// based on https://github.com/Azure-Samples/ms-identity-easyauth-dotnet-storage-graphapi/blob/main/3-WebApp-graphapi-managed-identity/Pages/Graph-MSI/Index.cshtml.cs
GraphServiceClient GraphServiceClient1()
{
    // Create the Graph service client with a ChainedTokenCredential which gets an access
    // token using the available Managed Identity or environment variables if running
    // in development.
    var credential = new ChainedTokenCredential(
        new ManagedIdentityCredential(),
        new EnvironmentCredential());
    var token = credential.GetToken(
        new Azure.Core.TokenRequestContext(
            new[] {"https://graph.microsoft.com/.default"}));

    var accessToken = token.Token;
    var graphServiceClient1 = new GraphServiceClient(
        new DelegateAuthenticationProvider((requestMessage) =>
        {
            requestMessage
                .Headers
                .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            return Task.CompletedTask;
        }));
    return graphServiceClient1;
}

async Task<List<MSGraphUser>> GetUsers()
{
    var graphServiceClient = GraphServiceClient1();

    var msGraphUsers = new List<MSGraphUser>();
    var users = await graphServiceClient.Users.Request().GetAsync();
    foreach (var u in users)
    {
        MSGraphUser user = new MSGraphUser();
        user.userPrincipalName = u.UserPrincipalName;
        user.displayName = u.DisplayName;
        user.mail = u.Mail;
        user.jobTitle = u.JobTitle;

        msGraphUsers.Add(user);
    }

    return msGraphUsers;
}

async Task<List<MSGraphServicePrincipal>> GetServicePrincipals()
{
    var graphServiceClient = GraphServiceClient1();

    var msGraphUsers = new List<MSGraphServicePrincipal>();
    var users = await graphServiceClient.ServicePrincipals.Request().GetAsync();
    foreach (var u in users)
    {
        MSGraphServicePrincipal user = new MSGraphServicePrincipal();
        user.appId = u.AppId;
        user.displayName = u.DisplayName;

        msGraphUsers.Add(user);
    }

    return msGraphUsers;
}


app.MapGet("/", () => "Hello World!");
app.MapGet("/users", GetUsers);
app.MapGet("/principals", GetServicePrincipals);

app.Run();