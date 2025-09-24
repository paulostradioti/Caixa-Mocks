using CreditAnalysis.Enums;

namespace CreditAnalysis
{
    public interface IRiskProvider
    {
        CreditRisk GetRisk(int? idade);
    }
}