using System.Collections.Generic;
using System.Xml.Linq;

namespace Myra.MML
{
    public delegate void MMLDiagnosticAction(MMLDiagnostic diagnostic);

    public class MMLDiagnostic
    {
        public List<XObject> TargetElements { get; } = new List<XObject>();
        public MMLDiagnosticSeverity Severity { get; }
        public string Group { get; }
        public string ID { get; }
        public string Message { get; }
        
        public MMLDiagnostic(MMLDiagnosticSeverity severity, string group, string id, string message)
        {
            Severity = severity;
            Group = group;
            ID = id;
            Message = message;
        }
    }
}
