using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Bluebird.FileManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Panda.DynamicWebApi;
using Panda.DynamicWebApi.Attributes;

namespace Bluebird.webapi.Controllers
{
    [DynamicWebApi]
    public class AppleAppService : IDynamicWebApi
    {
        private static readonly Dictionary<int, string> Apples = new Dictionary<int, string>() {
            [1] = "Big Apple",
            [2] = "Small Apple"
        };


        public void PostApple() {

           string Temp= File.ReadAllText("Template/File.txt");
           CreateDll("mycontroller", Temp);
        }

         public void CreateDll(string DllNamewithoutExt, string Template)
        {
            var bathPath = Directory.GetCurrentDirectory();
            string DllFullPath = Path.Combine(bathPath, DllNamewithoutExt);
            string DllFullName = Path.Combine(DllFullPath, DllNamewithoutExt + ".dll");
            //string DLLFullXML= Path.Combine(DllFullPath, DllNamewithoutExt + ".xml");
            if (!Directory.Exists(DllFullPath)) {
                Directory.CreateDirectory(DllFullPath);
            }
            else {
                Directory.Delete(DllFullPath, true);
                Directory.CreateDirectory(DllFullPath);
            }

            var _refef = DependencyContext.Default.GetDefaultAssemblyNames().ToList();
            var dd = _refef.Where(a => a.Name.Contains("Microsoft.AspNetCore.Mvc"));
            MetadataReference[] _ref = _refef.Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location)).ToArray();

            ///加载基础引用
            //MetadataReference[] _ref = DependencyContext.Default.CompileLibraries.
            //    Where(a => !a.Name.Equals("Microsoft.AspNetCore.Antiforgery") && !a.Name.Contains("Microsoft.AspNetCore")).
            //    SelectMany(a => a.ResolveReferencePaths().Select(b => MetadataReference.CreateFromFile(b))
            //    .ToArray()).ToArray();
            //       MetadataReference[] _ref =
            //DependencyContext.Default.CompileLibraries
            //.First(cl => cl.Name == "Microsoft.AspNetCore.App")
            //.ResolveReferencePaths()
            //.Select(asm => MetadataReference.CreateFromFile(asm))
            //.ToArray();

            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create(DllNamewithoutExt,  references: new[] { Mscorlib }, options: options)
               .WithOptions(new CSharpCompilationOptions(
                   Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                   usings: null,
                   optimizationLevel: OptimizationLevel.Release, // TODO
                   checkOverflow: true,                       // TODO
                   allowUnsafe: true,                          // TODO
                   platform: Platform.AnyCpu,
                   warningLevel: 4,
                   xmlReferenceResolver: null // don't support XML file references in interactive (permissions & doc comment includes)
                   ))
               .AddReferences(_ref)

             .AddSyntaxTrees(CSharpSyntaxTree.ParseText(Template, new CSharpParseOptions {

             }))
             ;
            var eResult = compilation.Emit(DllFullName);
            compilation.em
            var asms = AssemblyLoadContext.Default.LoadFromAssemblyPath(DllFullName);
        }

        /// <summary>
        /// Get An Apple.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public string Get(int id)
        {
            if (Apples.ContainsKey(id)) {
                return Apples[id];
            }
            else {
                return "No Apple!";
            }
        }

        /// <summary>
        /// Get All Apple.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Get()
        {
            return Apples.Values;
        }

        public void Update(UpdateAppleDto dto)
        {
            if (Apples.ContainsKey(dto.Id)) {
                Apples[dto.Id] = dto.Name;
            }
        }

        /// <summary>
        /// Delete Apple
        /// </summary>
        /// <param name="id">Apple Id</param>
        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
            if (Apples.ContainsKey(id)) {
                Apples.Remove(id);
            }
        }

    }
}
