using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Hiromi.Services.Eval
{
    public class EvalService : IEvalService
    {

        private readonly ScriptOptions _scriptOptions = ScriptOptions
            .Default
            .WithImports("System");
        
        public async Task<EvalResult> EvaluateAsync(string code)
        {
            try
            {
                var result = await CSharpScript.EvaluateAsync(code, _scriptOptions);
                return new EvalResult(result.ToString(), null);
            }
            catch (CompilationErrorException ex)
            {
                return new EvalResult(null, ex.Diagnostics);
            }
        }
    }
}