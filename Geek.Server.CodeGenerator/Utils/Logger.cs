using Microsoft.CodeAnalysis;

namespace Geek.Server.CodeGenerator.Utils
{
    public static class Logger
    {
        public static bool IsDebug = true;
        public static void LogError(this GeneratorExecutionContext context, string msg)
        {
            DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "Error",
                                                                                               title: "Code Generator Error",
                                                                                               messageFormat: "{0}",
                                                                                               category: "CodeGenerator",
                                                                                               DiagnosticSeverity.Error,
                                                                                               isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, msg));
        }
        
        public static void LogNormal(this GeneratorExecutionContext context, string msg)
        {
            if (!IsDebug)
            {
                return;
            }
            DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "Info",
                title: "Code Generator Info",
                messageFormat: "{0}",
                category: "CodeGenerator",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, msg));
        }
    }
}
