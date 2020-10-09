using System;
using Bluebird.FileManager;
using Xunit;

namespace Bluebird.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            GeneratEntity generat = new GeneratEntity();
            var data = generat.CreateEntityClass("CustomerManager", "Customer", new string[] { 
            "public int id{get;set;}","public string name{get;set;}",
            "public string code{get;set;}","public string number{get;set;}"
            });
            generat.CreateDll("Customer", data);
            generat.LoadDllInProject("Customer");
        }
    }
}
