using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Hiromi.Services.Eval
{
    public class EvalResult
    {
        public string Result { get; }
        public IEnumerable<Diagnostic> Diagnostics { get; }

        public EvalResult(string result, IEnumerable<Diagnostic> diagnostics)
        {
            Result = result;
            Diagnostics = diagnostics;
        }
    }
}