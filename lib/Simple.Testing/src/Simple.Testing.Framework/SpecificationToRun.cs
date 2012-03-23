using System;
using System.Reflection;
using Simple.Testing.ClientFramework;

namespace Simple.Testing.Framework
{
    public class SpecificationToRun
    {
        public readonly Specification Specification;
        public readonly MemberInfo FoundOn;
        public readonly bool IsRunnable;
        public readonly string Reason;
        public readonly Exception Exception;

        public SpecificationToRun(Specification specification, MemberInfo foundOn)
        {
            IsRunnable = true;
            Reason = "";
            Exception = null;
            Specification = specification;
            FoundOn = foundOn;
        }

        public SpecificationToRun(Specification specification, string reason, Exception exception, MemberInfo foundOn)
        {
            FoundOn = foundOn;
            Specification = specification;
            Exception = exception;
            Reason = reason;
            IsRunnable = false;
        }
    }
}