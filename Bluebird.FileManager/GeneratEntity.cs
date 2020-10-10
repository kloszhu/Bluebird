using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            else {
                Directory.Delete(DllFullPath, true);
                Directory.CreateDirectory(DllFullPath);
            }
            ///加载基础引用
            MetadataReference[] _ref = DependencyContext.Default.CompileLibraries.
                SelectMany(a => a.ResolveReferencePaths().Select(b => MetadataReference.CreateFromFile(b))
                .ToArray()).ToArray();

            var compilation = CSharpCompilation.Create(DllNamewithoutExt, references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) })
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

             .AddSyntaxTrees(CSharpSyntaxTree.ParseText(Template))
             ;
           var eResult = compilation.Emit(DllFullName);
           var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(DllFullName);
        }

       



    }
}
