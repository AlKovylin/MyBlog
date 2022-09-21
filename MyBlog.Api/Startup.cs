using AutoMapper;
using FluentValidation;
using MiBlog.Api.Contracts.Models.Users;
using MiBlog.Api.Contracts.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MyBlog.Domain.Core;
using MyBlog.Domain.Interfaces;
using MyBlog.Infrastructure.Data;
using MyBlog.Infrastructure.Data.Repository;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MyBlog.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            string connect = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(opton => opton.UseSqlite(connect));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IRepository<Comment>, CommentRepository>();
            services.AddTransient<IRepository<Tag>, TagRepository>();
            services.AddTransient<IRepository<Role>, RoleRepository>();

            services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterRequestValidator>();
            services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidator>();
            services.AddScoped<IValidator<UserUpdateRequest>, UserUpdateRequestValidator>();

            var mapperConfig = new MapperConfiguration((v) =>
                {
                    v.AddProfile(new MappingProfile());
                });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MyBlog.Api",
                    Version = "v1",
                    Description = "Финальный проект ASP.NET Core Web API",
                    Contact = new OpenApiContact { Name = "Ковылин Алексей", Email = "alkovylin@ya.ru" },
                    License = new OpenApiLicense { Name = "Open Source" }
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            services.AddAuthentication(options => options.DefaultScheme = "Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyBlog.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
