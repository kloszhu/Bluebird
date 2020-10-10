using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Panda.DynamicWebApi;

namespace Bluebird.webapi
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
            services.AddControllers();
            // 注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Panda Dynamic WebApi", Version = "v1" });

                // TODO:一定要返回true！
                options.DocInclusionPredicate((docName, description) => true);

                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                var xmlFile = System.AppDomain.CurrentDomain.FriendlyName + ".xml";
                var xmlPath = Path.Combine(baseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    //options.Authority = "http://www.google.com";
                    //options.RequireHttpsMetadata = false;
                    //options.Audience = "Panda-Api";
                });
            //services.AddDynamicWebApi();

            // 自定义配置
            services.AddDynamicWebApi((options) => {
                // 指定全局默认的 api 前缀
                options.DefaultApiPrefix = "apis";

                /**
                 * 清空API结尾，不删除API结尾;
                 * 若不清空 CreatUserAsync 将变为 CreateUser
                 */
                options.RemoveActionPostfixes.Clear();

                /**
                 * 自定义 ActionName 处理函数;
                 */
                options.GetRestFulActionName = (actionName) => actionName;

                /**
                 * 指定程序集 配置 url 前缀为 apis
                 * 如: http://localhost:8080/apis/User/CreateUser
                 */
                options.AddAssemblyOptions(this.GetType().Assembly, apiPreFix: "apis");

                /**
                 * 指定程序集 配置所有的api请求方式都为 POST
                 */
                options.AddAssemblyOptions(this.GetType().Assembly, httpVerb: "POST");

                /**
                 * 指定程序集 配置 url 前缀为 apis, 且所有请求方式都为POST
                 * 如: http://localhost:8080/apis/User/CreateUser
                 */
                options.AddAssemblyOptions(this.GetType().Assembly, apiPreFix: "apis", httpVerb: "POST");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panda Dynamic WebApi");
            });
        }
    }
}
