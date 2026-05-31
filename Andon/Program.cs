using Andon.Helpers;
using Andon.Hubs;
using Andon.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Andon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //数据库链接
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddSignalR();
            builder.Services.AddScoped<JwtHelper>();
            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // 全局支持 TimeSpan 转换
                    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                }); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                //swagger 注释
                var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                // 给 Swagger 添加 JWT 认证按钮
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT 授权，格式：Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // 让 Swagger 自动带上 Token 请求
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
            });

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var secretKey = Encoding.UTF8.GetBytes(jwtSection["Secret"]);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        // 跳过默认的 401 响应
                        context.HandleResponse();

                        context.Response.StatusCode = 200; // 仍然返回 200，但在响应体中包含错误信息
                        context.Response.ContentType = "application/json";

                        string msg;
                        int code;

                        if (context.AuthenticateFailure?.InnerException is SecurityTokenExpiredException)
                        {
                            code = 4001;
                            msg = "登录已过期，请重新登录";
                        }
                        else
                        {
                            code = 4002;
                            msg = "未登录或Token无效";
                        }

                        var result = new { code, msg };
                        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
                    }

                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<LineMonitorHub>("/hub/linemonitor");
            app.MapHub<EquipmentRTDHub>("/hub/equipmentrtd");
            app.Run();
        }
    }
}
