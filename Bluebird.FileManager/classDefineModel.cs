using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bluebird.FileManager
{

    public class WriteClassModel { }

    public class classDefineModel
    {


        public string assemblyName { get; set; }
        public string moduleName { get; set; }
        public TypeAttributes typeAttributes { get; set; } = TypeAttributes.Public;
        public Type Praent { get; set; } = null;

        public Guid GUID { get; set; } = Guid.NewGuid();
        public string className { get; set; }
        public IEnumerable<fieldDefineModel> propertyDefineModels { get; set; }
        public IEnumerable<MethodDefineModel> methodDefineModels { get; set; }
    }

    public class fieldDefineModel
    {
        public string propertyName { get; set; }
        public Type propertyType { get; set; }
        public FieldAttributes fieldAttribute { get; set; } = FieldAttributes.Public;
        public List<CustomAttributeData> CustomAttributes { get; set; } = new List<CustomAttributeData>();
    }



    public class MethodDefineModel
    {

    }
}
