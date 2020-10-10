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
            // ע��Swagger������������һ���Ͷ��Swagger �ĵ�
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Panda Dynamic WebApi", Version = "v1" });

                // TODO:һ��Ҫ����true��
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
            services.AddDynamicWebApi();

            // �Զ�������
            //services.AddDynamicWebApi((options) =>
            //{
            //    // ָ��ȫ��Ĭ�ϵ� api ǰ׺
            //    options.DefaultApiPrefix = "apis";

            //    /**
            //     * ���API��β����ɾ��API��β;
            //     * ������� CreatUserAsync ����Ϊ CreateUser
            //     */
            //    options.RemoveActionPostfixes.Clear();

            //    /**
            //     * �Զ��� ActionName ������;
            //     */
            //    options.GetRestFulActionName = (actionName) => actionName;

            //    /**
            //     * ָ������ ���� url ǰ׺Ϊ apis
            //     * ��: http://localhost:8080/apis/User/CreateUser
            //     */
            //    options.AddAssemblyOptions(this.GetType().Assembly, apiPreFix: "apis");

            //    /**
            //     * ָ������ �������е�api����ʽ��Ϊ POST
            //     */
            //    options.AddAssemblyOptions(this.GetType().Assembly, httpVerb: "POST");

            //    /**
            //     * ָ������ ���� url ǰ׺Ϊ apis, ����������ʽ��ΪPOST
            //     * ��: http://localhost:8080/apis/User/CreateUser
            //     */
            //    options.AddAssemblyOptions(this.GetType().Assembly, apiPreFix: "apis", httpVerb: "POST");
            //});
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
            //�����м�������swagger-ui��ָ��Swagger JSON�ս��
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panda Dynamic WebApi");
            });
        }
    }
}
