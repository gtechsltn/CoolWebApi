const string SwaggerRoutePrefix = "docs";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddDataAnnotationsLocalization();
builder.Services.AddAndConfigLocalization()
    .AddAndConfigApiVersioning()
    .AddAndConfigSwagger()
    .AddAndConfigWeatherHttpClient(builder.Configuration);

builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

var app = builder.Build();

app.UseApiExceptionHandling();

app.UseRequestLocalization();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = $"{SwaggerRoutePrefix}/{{documentName}}/docs.json"; });
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = SwaggerRoutePrefix;
        foreach (var description in app.DescribeApiVersions())
            options.SwaggerEndpoint($"/{SwaggerRoutePrefix}/{description.GroupName}/docs.json", description.GroupName.ToUpperInvariant());
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();