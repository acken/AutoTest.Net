using System;
using EnvDTE;
using EnvDTE80;

namespace AutoTest.VS.Util
{
    public class MethodNameReader
    {
        public static string GetMethodStringFromElement(CodeElement elem)
        {
            try
            {
                if (elem.Kind == vsCMElement.vsCMElementFunction)
                {
                    return GetMethodStringFromElement(elem as CodeFunction);
                }
                if (elem.Kind == vsCMElement.vsCMElementProperty)
                {
                    var getter = ((CodeProperty) elem).Getter;
                    return GetMethodStringFromElement(getter);
                }
                if (elem.Kind == vsCMElement.vsCMElementVariable)
                {
                    return GetFieldStringFromElement(elem);
                }
            }
            catch(Exception ex)
            {
                Core.DebugLog.Debug.WriteDebug("Exception getting Method String : " + ex.ToString());
                return null;
            }
            return null;
        }

        private static string GetMethodStringFromElement(CodeFunction justfunction)
        {
            string all = null;
            var first = true;
            if (justfunction != null)
            {
                if (justfunction.FunctionKind == vsCMFunction.vsCMFunctionPropertySet)
                    return GetSetterNameFrom(justfunction);
                all = GetReturnType(justfunction) + " " + GetMethodName(justfunction) + "(";
                foreach (CodeParameter2 param in justfunction.Parameters)
                {
                    if (!first) all += ",";
                    var type = param.Type.AsFullName;
                    if (param.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                        type = getArray(param.Type);
                    type = GenericNameMangler.MangleParameterName(type);
                    if(param.ParameterKind == vsCMParameterKind.vsCMParameterKindOut || param.ParameterKind == vsCMParameterKind.vsCMParameterKindRef)
                    {
                        type += "&";
                    }
                    all += type;
                    first = false;
                }
                all += ")";
            }
            return all;
        }

        private static string GetSetterNameFrom(CodeFunction justfunction)
        {
            var ret = "System.Void ";
            ret += GetMethodName(justfunction) + "(";

            var type = justfunction.Type.AsFullName;
            if (justfunction.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                type = getArray(justfunction.Type);
            type = GenericNameMangler.MangleParameterName(type);
            ret += type + ")";
            return ret;
        }


        private static string GetFieldStringFromElement(CodeElement elem)
        {
            var v = (CodeVariable) elem;
            var typeName = GenericNameMangler.MangleTypeName(GetTypeName(v.Parent));
            var oftype = GetVariableType(v);
            return oftype + " " + typeName + "::" + v.Name;
        }

        private static string GetMethodName(CodeFunction function)
        {
            var typeName = GetTypeName(function.Parent);
            var typename = GenericNameMangler.MangleTypeName(typeName);
            string method;
            switch (function.FunctionKind)
            {
                case vsCMFunction.vsCMFunctionConstructor:
                    method = function.IsShared ? ".cctor" : ".ctor";
                    break;
                case vsCMFunction.vsCMFunctionDestructor:
                    method = "Finalize";
                    break;
                case vsCMFunction.vsCMFunctionPropertyGet:
                    method = "get_" + function.Name;
                    break;
                case vsCMFunction.vsCMFunctionPropertySet:
                    method = "set_" + function.Name;
                    break;
                default:
                    method = GenericNameMangler.MangleMethodName(function.Name);
                    break;
            }
            AutoTest.Core.DebugLog.Debug.WriteDebug("Method name is " + typename + "::" + method);
            return typename + "::" + method;
        }


        private static string GetTypeName(object item)
        {
            var type = item as CodeElement;
            if (type.Kind == vsCMElement.vsCMElementInterface)
                return getInterfaceName((CodeInterface) type);
            if (type.Kind == vsCMElement.vsCMElementStruct)
                return getStructName((CodeStruct2) type);
            if (type.Kind == vsCMElement.vsCMElementClass)
                return getClassName((CodeClass)type);
            if (type.Kind == vsCMElement.vsCMElementProperty)
                return GetTypeName(((CodeProperty) type).Parent);
            return null;
        }

        private static string getInterfaceName(CodeInterface @interface)
        {
            var parents = GetParents(@interface);
            var nspace = GetNameSpaceName(@interface.Namespace);
            return nspace + "." + parents;
        }

        private static string getClassName(CodeClass clazz)
        {
            string parents = GetParents(clazz);
            var nspace = GetNameSpaceName(clazz.Namespace);
            return nspace + "." + parents;
        }

        private static string GetNameSpaceName(CodeNamespace codeNamespace)
        {
            if(codeNamespace.Parent != null)
            {
                var parentNameSpace = codeNamespace.Parent as CodeNamespace;
                string current = codeNamespace.Name;
                if(parentNameSpace != null)
                {
                    current = GetNameSpaceName(parentNameSpace) + "." + current;
                }
                return current;
            }
            return codeNamespace.Name;
        }

        private static string getStructName(CodeStruct structure)
        {
            string parents = GetParents(structure);
            var nspace = GetNameSpaceName(structure.Namespace);
            return nspace + "." + parents;
        }

        public static string GetParents(CodeInterface c)
        {
            var parent = c.Parent as CodeClass2;
            return GetParents((CodeElement2)c, parent);
        }

        public static string GetParents(CodeClass c)
        {

            var parent = c.Parent as CodeClass;
            return GetParents((CodeElement)c, parent);
        }

        public static string GetParents(CodeStruct c)
        {
            var parent = c.Parent as CodeClass;
            return GetParents((CodeElement)c, parent);
        }

        private static string GetParents(CodeElement c, CodeClass parent)
        {
            if (parent != null)
            {
                var name = GetParents(parent) + "/" + GetTypeNameWithoutNamespace(c.FullName);
                return name;
            }
            return GetTypeNameWithoutNamespace(c.FullName);
        }

        private static string GetTypeNameWithoutNamespace(string fullName)
        {
            var lastdot = fullName.LastIndexOf(".");
            if (lastdot == -1) return fullName;
            return fullName.Substring(lastdot + 1, fullName.Length - lastdot - 1);
        }

        private static string GetReturnType(CodeFunction n)
        {
            if (n.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                return GenericNameMangler.MangleParameterName(getArray(n.Type));
            if (n.Type.AsFullName == "") return "System.Void";
            return GenericNameMangler.MangleParameterName(n.Type.AsFullName);
        }

        private static string GetVariableType(CodeVariable n)
        {
            if (n.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                return GenericNameMangler.MangleParameterName(getArray(n.Type));
            return GenericNameMangler.MangleParameterName(n.Type.AsFullName);
        }

        public class Foo
        {
            public int Bar()
            {
                return 2;
            }
        }

        private static string getArray(CodeTypeRef type)
        {
            try
            {
                return type.CodeType.FullName;
            }
            catch
            {
                return type.AsString;
            }
        }

    }
}
