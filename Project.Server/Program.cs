using MySqlConnector;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using System.Text;
using Project.Backend.Core;

namespace Project.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddTransient<IDbConnection, MySqlConnection>();
            builder.Services.AddTransient<IUsersService, UsersService>();
            builder.Services.AddTransient<IQuizService, QuizService>();
            builder.Services.AddTransient<IQuestionService, QuestionService>();
            builder.Services.AddControllers();

            // add authentication and authorization
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            // add authentication and specify the JWT scheme to check tokens against
            builder.Services
                   .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       //var jwtSettings = app.Services.GetRequiredService<IOptions<JwtSettings>>().Value;
                       options.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           ValidIssuer = "Issuer",
                           ValidAudience = "Audience",
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenGenerator.SecurityKey)),
                       };

                       options.Events = new JwtBearerEvents
                       {
                           OnChallenge = context => // event triggered when authentication is not successful for whatever reason
                           {
                               context.HandleResponse(); // prevent the default 401 response
                                                         // set the response status code to 401 Unauthorized
                               context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                               context.Response.ContentType = "application/problem+json";
                               return context.Response.WriteAsync("authentication error!");
                           }
                       };
                   });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}