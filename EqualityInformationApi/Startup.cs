using Hackney.Core.Sns;
using EqualityInformationApi.V1.Factories;
using Amazon;
using Amazon.XRay.Recorder.Core;
using Microsoft.Extensions.DependencyInjection;
using Hackney.Core.JWT;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Builder;
using EqualityInformationApi.V1.UseCase;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Hackney.Core.Middleware.Logging;
using Hackney.Core.Middleware.CorrelationId;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Hackney.Core.Logging;
using Hackney.Core.HealthCheck;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using EqualityInformationApi.V1.Boundary.Request.Validation;
using Microsoft.Extensions.Configuration;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Hackney.Core.Validation.AspNet;
using Hackney.Core.DynamoDb.HealthCheck;
using EqualityInformationApi.V1.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using Hackney.Core.DynamoDb;
using SnsInitilisationExtensions = Hackney.Core.Sns.SnsInitilisationExtensions;
using Hackney.Core.Http;
using Microsoft.Extensions.Hosting;
using EqualityInformationApi.Versioning;

namespace EqualityInformationApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string ApiName = "Equality Information Api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AWSSDKHandler.RegisterXRayForAllServices();
        }

        public IConfiguration Configuration { get; }
        private static List<ApiVersionDescription> _apiVersions { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddFluentValidation(Assembly.GetAssembly(typeof(EqualityInformationObjectValidator)));

            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified =
                    true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader =
                    new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddDynamoDbHealthCheck<EqualityInformationDb>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Token",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Equality Information API",
                        Name = "X-Api-Key",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Token"}
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in _apiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion.ToString()}";
                    c.SwaggerDoc(version,
                        new OpenApiInfo
                        {
                            Title = $"{ApiName}-api {version}",
                            Version = version,
                            Description =
                                $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                        });
                }

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            services.ConfigureLambdaLogging(Configuration);

            AWSXRayRecorder.InitializeInstance(Configuration);
            AWSXRayRecorder.RegisterLogger(LoggingOptions.SystemDiagnostics);

            services.AddLogCallAspect();

            services.ConfigureDynamoDB();

            SnsInitilisationExtensions.ConfigureSns(services);

            services.AddTokenFactory();
            services.AddHttpContextWrapper();

            services.AddScoped<ISnsFactory, SnsFactory>();

            RegisterGateways(services);
            RegisterUseCases(services);

            services.AddSnsGateway()
                .AddTokenFactory()
                .AddHttpContextWrapper();
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IEqualityInformationGateway, EqualityInformationGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<ICreateUseCase, CreateUseCase>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("x-correlation-id"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCorrelationId();
            app.UseLoggingScope();
            app.UseCustomExceptionHandler(logger);
            app.UseXRay("equality-information-api");

            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            _apiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in _apiVersions)
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
            });

            app.UseSwagger();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHealthChecks("/api/v1/healthcheck/ping",
                    new HealthCheckOptions { ResponseWriter = HealthCheckResponseWriter.WriteResponse });
            });
            app.UseLogCall();
        }
    }
}
