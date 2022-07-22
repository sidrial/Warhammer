using System;
using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Warhammer.Domain.Ladder;
using Warhammer.Domain.Users;
using Warhammer.Domain.Users.Entities;
using Warhammer.Infrastructure.Users;
using Warhammer.Domain.Tournaments;
using Warhammer.Infrastructure.Ladder;
using Warhammer.Infrastructure.Tournaments;
using User = Warhammer.Domain.Users.Entities.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc();

// DI (repos)
const string databaseName = "Warhammer";
var cosmosClient = InitializeCosmos();
builder.Services.AddSingleton<ITournamentRepo>(InitializeTournamentRepo(cosmosClient, "Tournament"));
builder.Services.AddSingleton<IUserRepo>(InitializeUserRepo(cosmosClient, "User"));
builder.Services.AddSingleton<ILadderRepo>(InitializeLadderRepo(cosmosClient, "Ladder"));

// Identity framework
builder.Services.AddTransient<IUserStore<User>, UserStore>();
builder.Services.AddTransient<IRoleStore<Role>, RoleStore>();
builder.Services.AddIdentity<User, Role>(options =>
    {
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
    })
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore>()
    .AddRoleStore<RoleStore>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Warhammer";
    options.AccessDeniedPath = "/identity/access-denied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/identity/login";
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Add application services.
builder.Services.AddSingleton<IEmailSender>(InitializeMailer());
builder.Services.AddTransient<IPasswordGenerator, PasswordGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute().RequireAuthorization();
    endpoints.MapControllers();
});
app.Run();

static TournamentRepo InitializeTournamentRepo(CosmosClient client, string containerName)
{
    var tournamentRepo = new TournamentRepo(client, databaseName, containerName);
    return tournamentRepo;
}

static UserRepo InitializeUserRepo(CosmosClient client, string containerName)
{
    var userCosmosRepo = new UserRepo(client, databaseName, containerName);
    return userCosmosRepo;
}

static LadderRepo InitializeLadderRepo(CosmosClient client, string containerName)
{
    var ladderRepo = new LadderRepo(client, databaseName, containerName);
    return ladderRepo;
}

CosmosClient InitializeCosmos()
{
    var apiKey = builder.Configuration["CosmosDb:ApiKey"];
    var connectionString = builder.Configuration["CosmosDb:ConnectionString"];
    var client = new CosmosClient(connectionString, apiKey);
    return client;
}

EmailSender InitializeMailer()
{
	var apiKey = builder.Configuration["SendGrid:ApiKey"];
	var replyToEmail = builder.Configuration["SendGrid:ReplyToEmail"];
	var client = new EmailSender(apiKey, replyToEmail);
	return client;
}
