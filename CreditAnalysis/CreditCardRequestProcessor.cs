using CreditAnalysis.CreditCardRequest;
using CreditAnalysis.Interfaces;

namespace CreditAnalysis
{
    public class CreditCardRequestProcessor
    {
        private readonly ICreditScoreProvider creditScoreProvider;
        private readonly IRiskProvider riskProvider;
        private readonly IDependencyService dependencyService;

        public CreditCardRequestProcessor(ICreditScoreProvider creditScoreProvider, IRiskProvider riskProvider, IDependencyService dependencyService)
        {
            this.creditScoreProvider = creditScoreProvider;
            this.riskProvider = riskProvider;
            this.dependencyService = dependencyService;
        }

        public AnalysisResponse Process(AnalysisRequest request)
        {
            var score = creditScoreProvider.GetScoreByCpf(request?.CPF);
            if (score <= 300)
                return AnalysisResponse.Rejected;

            if (score > 950)
                return AnalysisResponse.Approved;

            var risk = riskProvider.GetRisk(request?.Idade);
            if (risk == Enums.CreditRisk.Low)
                return AnalysisResponse.Approved;

            if (risk == Enums.CreditRisk.High)
                return AnalysisResponse.Rejected;

            if (dependencyService.Objeto1.Objeto2.IsVip)
                return AnalysisResponse.Approved;


            return AnalysisResponse.Rejected;
        }
    }
}
