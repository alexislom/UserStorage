using System;
using System.Reflection;
using System.Configuration;
using NLog;
using PostSharp.Aspects;

namespace UserService.AOP
{
    [Serializable]
    internal class LogMethodAttribute : OnMethodBoundaryAspect
    {
        #region Fields & properties 

        public static Logger Logger = LogManager.GetCurrentClassLogger();

        private string _nameOfMethod;
        private Type _nameOfClass;

        #endregion

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            _nameOfMethod = method.Name;
            _nameOfClass = method.DeclaringType;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["isLogged"]))
                Logger.Trace($"Сlass {_nameOfClass} call method {_nameOfMethod}");
        }

        public override void OnException(MethodExecutionArgs args)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["isLogged"]))
                Logger.Error($"Сlass {_nameOfClass} fall with exception {args.Exception}, in method {_nameOfMethod}");
        }
    }
}
