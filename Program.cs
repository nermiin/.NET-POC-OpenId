var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "oidc";
})
.AddJwtBearer("Bearer", options =>
{
    // Configure JWT bearer authentication options here
});
builder.Services.AddCors(options =>
    {
        options.AddPolicy("MyCorsPolicy", builder =>
        {
            builder.WithOrigins("http://localhost:5164") // Replace with your own allowed origins
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
builder.Services.AddOpenIddict()
    .AddServer(options =>
        {
            // Enable the token endpoint.
            options.SetTokenEndpointUris("connect/token");

            // Enable the client credentials flow.
            options.AllowClientCredentialsFlow();

            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();

            // Register the ASP.NET Core host and configure the ASP.NET Core options.
            options.UseAspNetCore()
                   .EnableTokenEndpointPassthrough();
        })  // Register the OpenIddict validation components.
        .AddValidation(options =>
        {
            // Import the configuration from the local OpenIddict server instance.
            options.UseLocalServer();

            // Register the ASP.NET Core host.
            options.UseAspNetCore();
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MyCorsPolicy");


app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();