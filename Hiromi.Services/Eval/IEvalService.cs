using System.Threading.Tasks;

namespace Hiromi.Services.Eval
{
    public interface IEvalService
    {
        Task<EvalResult> EvaluateAsync(string code);
    }
}