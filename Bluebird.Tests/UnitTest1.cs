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
using Bluebird.FileManager;

namespace Bluebird.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
       
            MetadataReference[] _ref = DependencyContext.Default.CompileLibraries.
            SelectMany(a => a.ResolveReferencePaths().Select(b => MetadataReference.CreateFromFile(b))
            .ToArray()).ToArray();
            GeneratEntity generat = new GeneratEntity();
            var data = generat.CreateEntityClass("CustomerManager", "Customer", new string[] { 
            "public int id{get;set;}","public string name{get;set;}",
            "public string code{get;set;}","public string number{get;set;}","public List<Student> Students{get;set;}=new List<Student>();"
            },new string[] { "Bluebird.Tests" });
            generat.CreateDll("Customer", data);
    
        }
    }
}
