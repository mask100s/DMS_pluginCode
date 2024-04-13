using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamManagementSystemPlugins
{
    public static class ErrorLog
    {
        public static void CreateExceptionLog(IOrganizationService service, Exception objException)
        {
            Entity onErrorRecord = new Entity(ErrorLogs.logicalName);
            if (objException.Message != null)
                onErrorRecord[ErrorLogs.errorMessage] = objException.Message.ToString();
            if (objException.Source != null)
                onErrorRecord[ErrorLogs.exceptionSource] = objException.Source.ToString();
            if (objException.StackTrace != null)
                onErrorRecord[ErrorLogs.stackTrace] = objException.StackTrace.ToString();

            service.Create(onErrorRecord);
        }

        internal static void CreateExceptionLog(IOrganizationService service, Exception objException, string empty, string strPluginName)
        {
            Entity onErrorRecord = new Entity(ErrorLogs.logicalName);
            if (objException.Message != null)
                onErrorRecord[ErrorLogs.errorMessage] = objException.Message.ToString();
            if (objException.Source != null)
                onErrorRecord[ErrorLogs.exceptionSource] = objException.Source.ToString();
            if (strPluginName != null)
                onErrorRecord[ErrorLogs.plugin] = strPluginName;
            if (strPluginName != null)
                onErrorRecord[ErrorLogs.stackTrace] = objException.StackTrace.ToString();

            service.Create(onErrorRecord);
        }
    }
}
    