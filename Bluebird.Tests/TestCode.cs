using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Bluebird.Tests
{
    public class TestCode
    {
        [Fact]
        void Test()
        {
            if (!Directory.Exists("dd")) {
                Directory.CreateDirectory("dd");
            }
            else {
                Directory.Delete("dd", true);
                Directory.CreateDirectory("dd");
            }
            ///加载基础引用
            MetadataReference[] _ref = DependencyContext.Default.CompileLibraries.
                SelectMany(a => a.ResolveReferencePaths().Select(b => MetadataReference.CreateFromFile(b))
                .ToArray()).ToArray();

            var vv = _ref.Where(a => a.Display.Contains("System.Reflection"));



            string testClass = @"
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Bluebird.Tests;
namespace CustomerManager 
{
public class Customer
{
public int id{get;set;}
public string name{get;set;}
public string code{get;set;}
public string number{get;set;}
}
}



";

            var compilation = CSharpCompilation.Create("abc")
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

              .AddSyntaxTrees(CSharpSyntaxTree.ParseText(testClass))
              ;


            var eResult = compilation.Emit("E:/Project/FD/FD.xunit/bin/Debug/netcoreapp3.1/dd/test.dll");

            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath("E:/Project/FD/FD.xunit/bin/Debug/netcoreapp3.1/dd/test.dll");
            var dds = Type.GetType("test.tes, test");
            var dpsssp = Type.GetType("System.Collections.Generic.List`1[[test.tes, test]]");
        }
    }
}
