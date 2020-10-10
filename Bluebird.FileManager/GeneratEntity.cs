using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;

namespace Bluebird.FileManager
{
    public class GeneratEntity
    {
        //AssemblyLoadContext.Default.LoadFromAssemblyPath("E:/Project/FD/FD.xunit/bin/Debug/netcoreapp3.1/dd/test.dll");

        private static string LeftQ="{";
        private static string RightQ = "}";
        public string CreateEntityClass(string modelName,string className ,string[] Fields=null, string[] Reflences=null ) {
            StringBuilder Template = new StringBuilder();
            Template.AppendLine(@"
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;");
            Reflences = Reflences==null? new HashSet<string>().ToArray(): Reflences;
            foreach (var item in Reflences) {
                Template.AppendLine($"using {item};");
            };
            Template.AppendLine($"namespace {modelName} ");
            Template.AppendLine(LeftQ);
            Template.AppendLine($"public class {className}");
            Template.AppendLine(LeftQ);
            Fields = Fields==null? new HashSet<string>().ToArray(): Fields;
            foreach (var item in Fields) {
                Template.AppendLine(item);
            }

            Template.AppendLine(RightQ);
            Template.AppendLine(RightQ);
            return Template.ToString();
        }



        public void CreateDll(string DllNamewithoutExt,string Template) {
            var bathPath = Directory.GetCurrentDirectory();
            string DllFullPath = Path.Combine(bathPath, DllNamewithoutExt);
            string DllFullName = Path.Combine(DllFullPath, DllNamewithoutExt + ".dll");
            //string DLLFullXML= Path.Combine(DllFullPath, DllNamewithoutExt + ".xml");
            if (!Directory.Exists(DllFullPath)) {
                Directory.CreateDirectory(DllFullPath);
            }
            else 
            {
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


   

            var compilation = CSharpCompilation.Create(DllNamewithoutExt)
               .WithOptions(new CSharpCompilationOptions(
                   Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                   usings: null,
                   optimizationLevel: OptimizationLevel.Debug, // TODO
                   checkOverflow: true,                       // TODO
                   allowUnsafe: true,                          // TODO
                   platform: Platform.AnyCpu,
                   warningLevel: 4,
                   xmlReferenceResolver: null // don't support XML file references in interactive (permissions & doc comment includes)
                   ))
               .AddReferences(_ref)

             .AddSyntaxTrees(CSharpSyntaxTree.ParseText(Template,new CSharpParseOptions {
            
             }))
             ;
           var eResult = compilation.Emit(DllFullName);
           var asms = AssemblyLoadContext.Default.LoadFromAssemblyPath(DllFullName);
        }

       



    }
}
