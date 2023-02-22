using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Timers;

namespace Simple
{
    public class LogTest
    {
        private readonly ILogger<LogTest> _logger;

        public LogTest(ILogger<LogTest> logger)
        {
            _logger = logger;
            var message = $" LogTest logger created at {DateTime.UtcNow.ToLongTimeString()}";
            _logger.LogInformation(message);
        }
    }

    public class Program
    {
        private static System.Timers.Timer aTimer;
        public   ILogger<LogTest>? logger;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecks();
            builder.Services.AddSingleton<MetricCollector>();


            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // Add services to the container.
            builder.Services.AddRazorPages();
            // Add services to the container.
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec-ch-ua");
                logging.ResponseHeaders.Add("MyResponseHeader");
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;

            });

           // foreach (var service in builder.Services)
           // {
           //     var hj = service.ImplementationType; //.ToString();
              //  Log.Debug(hj.ToString()); //.Log(hj);
           // };
             

            builder.Services.AddControllersWithViews();

            /*
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudience = "false",
                    ValidIssuer = "false",
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey))
                };

            });*/


            /*
             * add kestrel as below
             * docker-compose - add ports 443:443
             * launchSettings mean nothing
             * right click docker-compose -- set launch there to https://abc
             * register CARoot on windows in Personal - trusted
             * *************************************************
             * for the build on compose
             * cmd 
             * cd $(SolutionDir)
             * kompose.exe convert
             * exit
             * *****************************************
             *          [HttpGet(Name = "GetStudents")]
                        public string GetStudents()
                        {
                            List<Student> students1 = new List<Student>();
                            var students = from s in _context.Students
                                           select s;
                            var ls = students.ToList();
                            IQueryable<Student> studentsIQ = from s in _context.Students
                                                             select s;
             
                            students1 = studentsIQ.ToList();
                            var json = JsonConvert.SerializeObject(students1);
             
                            return json;
                        }
             * 
             * ***************************************************************************
             *          [HttpGet]
                        public string getJson()
                        {
                            AuthorList.Add(new Author("Mahesh Chand", 35, "A Prorammer's Guide to ADO.NET", true, new DateTime(2003, 7, 10)));
                            AuthorList.Add(new Author("Neel Beniwal", 18, "Graphics Development with C#", false, new DateTime(2010, 2, 22)));
                            AuthorList.Add(new Author("Praveen Kumar", 28, "Mastering WCF", true, new DateTime(2012, 01, 01)));
                            AuthorList.Add(new Author("Mahe99sh Chander", 35, "Graphics Programming with GDI+", true, new DateTime(2008, 01, 20)));
                            AuthorList.Add(new Author("Raj Kumaronski", 30, "Building Creative Systems", false, new DateTime(2011, 6, 3)));
                            string json = JsonConvert.SerializeObject(AuthorList);
                            return json;
                        }
             * 
             * 
             * 
             * [Host("*:81")]
public class My81Controller : Controller
{
  // stuff
}
             */


            builder.WebHost.ConfigureKestrel(options =>
             {
                 var port = 443;
                 //var pfxFilePath = @"/app/davetest.pfx";
                 var pfxFilePath = @"./davetest.pfx";
                 // The password you specified when exporting the PFX file using OpenSSL.
                 // This would normally be stored in configuration or an environment variable;
                 // I've hard-coded it here just to make it easier to see what's going on.
                 var pfxPassword = "";

                 options.Listen(IPAddress.Any, port, listenOptions =>
                 {
                     // Enable support for HTTP1 and HTTP2 (required if you want to host gRPC endpoints)
                     listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                     // Configure Kestrel to use a certificate from a local .PFX file for hosting HTTPS
                     listenOptions.UseHttps(pfxFilePath, pfxPassword);
                 });

                 
                 options.Listen(IPAddress.Any, 5000, listenOptions =>
                 {
                     // Enable support for HTTP1 and HTTP2 (required if you want to host gRPC endpoints)
                     listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                     // Configure Kestrel to use a certificate from a local .PFX file for hosting HTTPS
                     
                     //listenOptions.UseHttps(pfxFilePath, pfxPassword);
                 });
                 
             });
            
            var app = builder.Build();


            app.UseRequestMiddleware();
            app.UseGaugeMiddleware();


            app.UseMetricServer(5000, "/metrics");

            var diagnosticSource = app.Services.GetRequiredService<DiagnosticListener>();
            using var badRequestListener = new BadRequestEventListener(diagnosticSource, (badRequestExceptionFeature) =>
            {
                app.Logger.LogError(badRequestExceptionFeature.Error, "Bad request received");
            });


            ILogger<LogTest> logger = app.Services.GetRequiredService<ILogger<LogTest>>();
            // var test = new LogTest(loggerX);

            app.UseHttpLogging();
            

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                app.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");
                app.MapControllerRoute(
                  name: "listen81",
                  pattern: "{controller=Listen81}/{action=Index}/{id?}");
            });
            app.MapRazorPages();

            //  app.MapControllers();
            /*
           app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

          app.UseEndpoints(routes =>
          {
              routes.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");

              routes.MapControllerRoute(
                  name: "listen81",

                      pattern: "{controller=Home}/{action=Index}/{id?}");
              routes.MapRazorPages();
          });
          */

            //SetTimer();

            // Singleton cd = Singleton.Instance;
            // bool op = cd.KissAss();
            app.Run();
        }


 


        public void ProcessAndLog( )
        {
            //Program pc = new Program();
            //pc.logger.Log(LogLevel.Information, "krap");
        }


        private static  void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Program pc = new Program(); 
            pc.ProcessAndLog();
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }


        private static  void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
    }
}