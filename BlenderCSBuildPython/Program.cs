using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using BlenderCS.Attributes;

namespace BlenderCSBuildPython
{
    public class Program
    {
        private enum SeparationEnum
        {
            NONE,
            SINGLE,
            DOUBLE
        }

        private class EntryPointClassData
        {
            private Type _type;
            private EntryPointClassAttribute _entryPointClass;

            public Type Type { get => _type; set => _type = value; }
            public EntryPointClassAttribute EntryPointClass { get => _entryPointClass; set => _entryPointClass = value; }
        }

        private class PropertyGroupData
        {
            private Type _type;
            private PropertyGroupAttribute _propertyGroup;

            public Type Type { get => _type; set => _type = value; }
            public PropertyGroupAttribute PropertyGroup { get => _propertyGroup; set => _propertyGroup = value; }
        }

        private class OperatorData
        {
            private Type _type;
            private OperatorAttribute _operator;

            public Type Type { get => _type; set => _type = value; }
            public OperatorAttribute Operator { get => _operator; set => _operator = value; }
        }

        private class PanelData
        {
            private Type _type;
            private PanelAttribute _panel;

            public Type Type { get => _type; set => _type = value; }
            public PanelAttribute Panel { get => _panel; set => _panel = value; }
        }

        static string _sourceCode = "";

        static void Main(string[] args)
        {
            FileInfo fileInfo = new FileInfo(args[0]);

            Assembly dllAssembly = Assembly.LoadFile(args[0]);
            AddonInfoAttribute addonInfo = dllAssembly.GetCustomAttribute<AddonInfoAttribute>();
            if (addonInfo == null)
            {
                Console.WriteLine("Addon Info not found.");
                return;
            }

            EntryPointClassData entryPointClassData = new EntryPointClassData();
            List<PropertyGroupData> propertyGroups = new List<PropertyGroupData>();
            List<OperatorData> operators = new List<OperatorData>();
            List<PanelData> panels = new List<PanelData>();
            try
            {
                foreach (Type type in dllAssembly.GetTypes())
                {
                    EntryPointClassAttribute entryPointClass = type.GetCustomAttribute<EntryPointClassAttribute>();
                    if (entryPointClass != null)
                    {
                        entryPointClassData.Type = type;
                        entryPointClassData.EntryPointClass = entryPointClass;
                    }

                    PropertyGroupAttribute propertyGroup = type.GetCustomAttribute<PropertyGroupAttribute>();
                    if (propertyGroup != null)
                    {
                        PropertyGroupData propertyGroupData = new PropertyGroupData();
                        propertyGroupData.Type = type;
                        propertyGroupData.PropertyGroup = propertyGroup;

                        propertyGroups.Add(propertyGroupData);
                    }

                    OperatorAttribute operatorAttribute = type.GetCustomAttribute<OperatorAttribute>();
                    if (operatorAttribute != null)
                    {
                        OperatorData operatorData = new OperatorData();
                        operatorData.Type = type;
                        operatorData.Operator = operatorAttribute;

                        operators.Add(operatorData);
                    }

                    PanelAttribute panelAttribute = type.GetCustomAttribute<PanelAttribute>();
                    if (panelAttribute != null)
                    {
                        PanelData panelData = new PanelData();
                        panelData.Type = type;
                        panelData.Panel = panelAttribute;

                        panels.Add(panelData);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (Exception inner in ex.LoaderExceptions)
                {
                    Console.WriteLine(inner.Message);
                }
            }

            _buildPropertyGroupsClasses(propertyGroups, fileInfo.DirectoryName);
            _buildOperatorsClasses(operators, fileInfo.DirectoryName, fileInfo.Name, addonInfo);
            _buildPanelsClasses(panels, fileInfo.DirectoryName, fileInfo.Name, addonInfo);

            _sourceCode = _appendSourceCode(_sourceCode, _buildBPYImport(), SeparationEnum.NONE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildCLRImport(), SeparationEnum.NONE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildEntryPointImportSourceCode(fileInfo.Name, entryPointClassData, addonInfo), SeparationEnum.SINGLE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildPropertyGroupsImportSourceCode(propertyGroups), SeparationEnum.SINGLE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildOperatorsImportSourceCode(operators), SeparationEnum.NONE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildPanelsImportSourceCode(panels), SeparationEnum.NONE);
            _sourceCode = _appendSourceCode(_sourceCode, addonInfo.BuildSourceCode(), SeparationEnum.DOUBLE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildClassesTupleSourceCode(propertyGroups, operators, panels), SeparationEnum.DOUBLE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildEntryPointDeclarationSourceCode(entryPointClassData), SeparationEnum.DOUBLE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildRegisterFunctionSourceCode(), SeparationEnum.DOUBLE);
            _sourceCode = _appendSourceCode(_sourceCode, _buildUnregisterFunctionSourceCode(), SeparationEnum.DOUBLE);

            string initFilePath = fileInfo.Directory + "\\__init__.py";
            FileStream file = null;
            if (File.Exists(initFilePath))
            {
                file = new FileStream(initFilePath, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                file = new FileStream(initFilePath, FileMode.Create, FileAccess.Write);
            }
            byte[] sourceCodeBytes = Encoding.UTF8.GetBytes(_sourceCode);
            file.Write(sourceCodeBytes, 0, sourceCodeBytes.Length);
            file.Close();

            Console.WriteLine("Your python code is done. :)");
            //Console.ReadKey();
        }

        static string _appendSourceCode(string sourceCode, string appendSourceCode, SeparationEnum separation = SeparationEnum.NONE)
        {
            string resultSourceCode = sourceCode;

            if (!string.IsNullOrEmpty(appendSourceCode))
            {
                if (!string.IsNullOrEmpty(sourceCode))
                {
                    if (separation == SeparationEnum.SINGLE)
                    {
                        resultSourceCode += "\n";
                    }
                    else if (separation == SeparationEnum.DOUBLE)
                    {
                        resultSourceCode += "\n\n";
                    }
                    resultSourceCode += "\n" + appendSourceCode;
                }
                else
                {
                    resultSourceCode = appendSourceCode;
                }
            }

            return resultSourceCode;
        }

        static void _buildOperatorsClasses(List<OperatorData> operators, string parentPath, string dllName, AddonInfoAttribute addonInfo)
        {
            foreach (OperatorData operatorData in operators)
            {
                string sourceCode = "";
                sourceCode = _appendSourceCode(sourceCode, _buildBPYImport(), SeparationEnum.NONE);
                sourceCode = _appendSourceCode(sourceCode, _buildCLRImport(), SeparationEnum.NONE);
                sourceCode = _appendSourceCode(sourceCode, _buildOperatorImportSourceCode(dllName, operatorData, addonInfo), SeparationEnum.SINGLE);
                sourceCode = _appendSourceCode(sourceCode, _buildOperatorClassSourceCode(operatorData), SeparationEnum.DOUBLE);
                string[] paths = operatorData.Type.FullName.Split('.');
                bool first = true;
                string fullPath = "";
                foreach (string path in paths)
                {
                    if (first)
                    {
                        first = false;
                        fullPath = parentPath;
                    }
                    else
                    {
                        fullPath += "\\" + path;
                    }
                }
                fullPath += ".py";

                Directory.CreateDirectory(new FileInfo(fullPath).DirectoryName);
                FileStream file = null;
                if (File.Exists(fullPath))
                {
                    file = new FileStream(fullPath, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    file = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                }
                byte[] sourceCodeBytes = Encoding.UTF8.GetBytes(sourceCode);
                file.Write(sourceCodeBytes, 0, sourceCodeBytes.Length);
                file.Close();
            }
        }

        static string _buildOperatorImportSourceCode(string dllName, OperatorData operatorData, AddonInfoAttribute addonInfo)
        {
            string sourceCode = "";

            sourceCode += "clr.AddReference(\"{0}\\\\addons\\\\" + addonInfo.Name + "\\\\" + dllName + "\".format(bpy.utils.user_resource('SCRIPTS')))";
            string pathPrev = null;
            string fromText = "";
            foreach (string path in operatorData.Type.FullName.Split('.'))
            {
                if (pathPrev != null)
                {
                    fromText += "." + pathPrev;
                }

                pathPrev = path;
            }
            sourceCode += string.Format("\nfrom {0} import {1} as {1}_CS", fromText, operatorData.Type.Name);

            return sourceCode;
        }

        static string _buildOperatorClassSourceCode(OperatorData operatorData)
        {
            string sourceCode = "";
            sourceCode += string.Format("class {0}(bpy.types.Operator):\n", operatorData.Type.Name);
            sourceCode = _appendSourceCode(sourceCode, operatorData.Operator.BuildSourceCode(), SeparationEnum.NONE);
            sourceCode += string.Format("\n\tdef __init__(self):\n");
            sourceCode += string.Format("\t\tself.cs_operator_instance = {0}_CS()\n\n", operatorData.Type.Name);
            sourceCode += string.Format("\tdef execute(self, context):\n");
            sourceCode += string.Format("\t\tresult = self.cs_operator_instance.Execute(self, context)\n", operatorData.Type.Name);
            sourceCode += "\t\treturn {result}";

            return sourceCode;
        }

        static void _buildPanelsClasses(List<PanelData> panels, string parentPath, string dllName, AddonInfoAttribute addonInfo)
        {
            foreach (PanelData panelData in panels)
            {
                string sourceCode = "";
                sourceCode = _appendSourceCode(sourceCode, _buildBPYImport(), SeparationEnum.NONE);
                sourceCode = _appendSourceCode(sourceCode, _buildCLRImport(), SeparationEnum.NONE);
                sourceCode = _appendSourceCode(sourceCode, _buildPanelImportSourceCode(dllName, panelData, addonInfo), SeparationEnum.SINGLE);
                sourceCode = _appendSourceCode(sourceCode, _buildPanelClassSourceCode(panelData), SeparationEnum.DOUBLE);
                string[] paths = panelData.Type.FullName.Split('.');
                bool first = true;
                string fullPath = "";
                foreach (string path in paths)
                {
                    if (first)
                    {
                        first = false;
                        fullPath = parentPath;
                    }
                    else
                    {
                        fullPath += "\\" + path;
                    }
                }
                fullPath += ".py";

                Directory.CreateDirectory(new FileInfo(fullPath).DirectoryName);
                FileStream file = null;
                if (File.Exists(fullPath))
                {
                    file = new FileStream(fullPath, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    file = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                }
                byte[] sourceCodeBytes = Encoding.UTF8.GetBytes(sourceCode);
                file.Write(sourceCodeBytes, 0, sourceCodeBytes.Length);
                file.Close();
            }
        }

        static string _buildPanelImportSourceCode(string dllName, PanelData panelData, AddonInfoAttribute addonInfo)
        {
            string sourceCode = "";

            sourceCode += "clr.AddReference(\"{0}\\\\addons\\\\" + addonInfo.Name + "\\\\" + dllName + "\".format(bpy.utils.user_resource('SCRIPTS')))";
            string pathPrev = null;
            string fromText = "";
            foreach (string path in panelData.Type.FullName.Split('.'))
            {
                if (pathPrev != null)
                {
                    fromText += "." + pathPrev;
                }

                pathPrev = path;
            }
            sourceCode += string.Format("\nfrom {0} import {1} as {1}_CS", fromText, panelData.Type.Name);

            return sourceCode;
        }

        static string _buildPanelClassSourceCode(PanelData panelData)
        {
            string sourceCode = "";
            sourceCode += string.Format("class {0}(bpy.types.Panel):\n", panelData.Type.Name);
            sourceCode = _appendSourceCode(sourceCode, panelData.Panel.BuildSourceCode(), SeparationEnum.NONE);
            sourceCode += string.Format("\n\tdef __init__(self):\n");
            sourceCode += string.Format("\t\tself.cs_panel_instance = {0}_CS()\n\n", panelData.Type.Name);
            sourceCode += string.Format("\tdef draw(self, context):\n");
            sourceCode += string.Format("\t\tself.cs_panel_instance.Draw(self, context)\n", panelData.Type.Name);

            return sourceCode;
        }

        static void _buildPropertyGroupsClasses(List<PropertyGroupData> propertyGroups, string parentPath)
        {
            foreach (PropertyGroupData propertyGroupData in propertyGroups)
            {
                string sourceCode = "";
                sourceCode = _appendSourceCode(sourceCode, _buildBPYImport(), SeparationEnum.NONE);
                sourceCode = _appendSourceCode(sourceCode, _buildPropertyGroupClassSourceCode(propertyGroupData), SeparationEnum.DOUBLE);
                string[] paths = propertyGroupData.Type.FullName.Split('.');
                bool first = true;
                string fullPath = "";
                foreach (string path in paths)
                {
                    if (first)
                    {
                        first = false;
                        fullPath = parentPath;
                    }
                    else
                    {
                        fullPath += "\\" + path;
                    }
                }
                fullPath += ".py";

                Directory.CreateDirectory(new FileInfo(fullPath).DirectoryName);
                FileStream file = null;
                if (File.Exists(fullPath))
                {
                    file = new FileStream(fullPath, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    file = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                }
                byte[] sourceCodeBytes = Encoding.UTF8.GetBytes(sourceCode);
                file.Write(sourceCodeBytes, 0, sourceCodeBytes.Length);
                file.Close();
            }
        }

        static string _buildPropertyGroupClassSourceCode(PropertyGroupData propertyGroupData)
        {
            string sourceCode = "";
            sourceCode += string.Format("class {0}(bpy.types.PropertyGroup):", propertyGroupData.Type.Name);
            foreach (PropertyInfo propertyInfo in propertyGroupData.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                sourceCode = _appendSourceCode(sourceCode, _buildPropertySourceCode(propertyInfo), SeparationEnum.NONE);
            }

            return sourceCode;
        }

        static string _buildPropertySourceCode(PropertyInfo propertyInfo)
        {
            string sourceCode = "";

            foreach (PropertyAttribute property in propertyInfo.GetCustomAttributes<PropertyAttribute>())
            {
                if (property is StringPropertyAttribute)
                {
                    StringPropertyAttribute stringProperty = (StringPropertyAttribute)property;
                    sourceCode += string.Format("\t{0}: bpy.props.StringProperty(", propertyInfo.Name, stringProperty.Name);
                    sourceCode += string.Format("name=\"{0}\",", stringProperty.Name);
                    sourceCode += string.Format("description=\"{0}\",", stringProperty.Description);
                    sourceCode += string.Format("default=\"{0}\",", stringProperty.Default);
                    sourceCode += string.Format("maxlen={0}", stringProperty.MaxLength);
                    sourceCode += ")";
                }
            }

            return sourceCode;
        }

        static string _buildBPYImport()
        {
            string sourceCode = "";

            sourceCode += "import bpy";

            return sourceCode;
        }

        static string _buildCLRImport()
        {
            string sourceCode = "";

            sourceCode += "import clr";

            return sourceCode;
        }

        static string _buildEntryPointImportSourceCode(string dllName, EntryPointClassData entryPointClass, AddonInfoAttribute addonInfo)
        {
            string sourceCode = "";

            sourceCode += "clr.AddReference(\"{0}\\\\addons\\\\" + addonInfo.Name + "\\\\" + dllName + "\".format(bpy.utils.user_resource('SCRIPTS')))";
            string pathPrev = null;
            string fromText = "";
            foreach (string path in entryPointClass.Type.FullName.Split('.'))
            {
                if (pathPrev != null)
                {
                    fromText += "." + pathPrev;
                }

                pathPrev = path;
            }
            sourceCode += string.Format("\nfrom {0} import {1}", fromText, entryPointClass.Type.Name);

            return sourceCode;
        }

        static string _buildPropertyGroupsImportSourceCode(List<PropertyGroupData> propertyGroups)
        {
            string sourceCode = "";

            bool first = true;
            foreach (PropertyGroupData propertyGroupData in propertyGroups)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sourceCode += "\n";
                }
                string fromText = "";
                string[] paths = propertyGroupData.Type.FullName.Split('.');
                first = true;
                foreach (string path in paths)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        fromText += "." + path;
                    }
                }

                sourceCode += string.Format("from {0} import {1}", fromText, propertyGroupData.Type.Name);
            }

            return sourceCode;
        }

        static string _buildOperatorsImportSourceCode(List<OperatorData> operators)
        {
            string sourceCode = "";

            bool first = true;
            foreach (OperatorData operatorData in operators)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sourceCode += "\n";
                }
                string fromText = "";
                string[] paths = operatorData.Type.FullName.Split('.');
                first = true;
                foreach (string path in paths)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        fromText += "." + path;
                    }
                }

                sourceCode += string.Format("from {0} import {1}", fromText, operatorData.Type.Name);
            }

            return sourceCode;
        }

        static string _buildPanelsImportSourceCode(List<PanelData> panels)
        {
            string sourceCode = "";

            bool first = true;
            foreach (PanelData panelData in panels)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sourceCode += "\n";
                }
                string fromText = "";
                string[] paths = panelData.Type.FullName.Split('.');
                first = true;
                foreach (string path in paths)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        fromText += "." + path;
                    }
                }

                sourceCode += string.Format("from {0} import {1}", fromText, panelData.Type.Name);
            }

            return sourceCode;
        }

        static string _buildClassesTupleSourceCode(List<PropertyGroupData> propertyGroups, List<OperatorData> operators, List<PanelData> panels)
        {
            string sourceCode = "";
            sourceCode += "classes = (\n";
            foreach (PropertyGroupData propertyGroupData in propertyGroups)
            {
                sourceCode += string.Format("\t{0},\n", propertyGroupData.Type.Name);
            }
            foreach (OperatorData operatorData in operators)
            {
                sourceCode += string.Format("\t{0},\n", operatorData.Type.Name);
            }
            foreach (PanelData panelData in panels)
            {
                sourceCode += string.Format("\t{0},\n", panelData.Type.Name);
            }
            sourceCode += ")\n";
            return sourceCode;
        }

        static string _buildEntryPointDeclarationSourceCode(EntryPointClassData entryPointClass)
        {
            string sourceCode = "";
            sourceCode += string.Format("entry_point = {0}()", entryPointClass.Type.Name);
            return sourceCode;
        }

        static string _buildRegisterFunctionSourceCode()
        {
            string sourceCode = "";
            sourceCode += "def register():\n";
            sourceCode += "\tentry_point.RegisterPre()\n";
            sourceCode += "\tfor cls in classes:\n";
            sourceCode += "\t\tbpy.utils.register_class(cls)\n";
            sourceCode += "\tentry_point.RegisterPost()";
            return sourceCode;
        }

        static string _buildUnregisterFunctionSourceCode()
        {
            string sourceCode = "";
            sourceCode += "def unregister():\n";
            sourceCode += "\tentry_point.UnregisterPre()\n";
            sourceCode += "\tfor cls in classes:\n";
            sourceCode += "\t\tbpy.utils.unregister_class(cls)\n";
            sourceCode += "\tentry_point.UnregisterPost()";
            return sourceCode;
        }
    }
}
