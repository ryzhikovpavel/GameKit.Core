using System.IO;
using System.Text;
using UnityEngine;

namespace GameKit.Editor.Tools
{
    public class ConstantFileBuilder: ConstantClass
    {
        private readonly string _namespaceName;

        public ConstantFileBuilder(string className, string namespaceName = null): base(className)
        {
            if (string.IsNullOrEmpty(namespaceName) == false)
                _namespaceName = GetCamelName(namespaceName);
        }

        public void Save(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, $"{ClassName}.cs");

            string offset = "";
            
            StringBuilder builder = new StringBuilder();
            if (_namespaceName.IsNotEmpty())
            {
                builder.AppendLine($"namespace {_namespaceName}");
                builder.AppendLine("{");
                offset = "\t";
            }
            
            builder.AppendLine(GenerateCode(offset));
            
            if (_namespaceName.IsNotEmpty())
                builder.AppendLine("}");

            File.WriteAllText(path, builder.ToString());
        }
    }
}