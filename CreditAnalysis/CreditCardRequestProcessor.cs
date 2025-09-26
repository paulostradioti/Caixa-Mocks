using CreditAnalysis.CreditCardRequest;
using CreditAnalysis.Interfaces;
using System.Diagnostics;

namespace CreditAnalysis
{
    public class CreditCardRequestProcessor
    {
        private readonly ICreditScoreProvider creditScoreProvider;
        private readonly IRiskProvider riskProvider;
        private readonly IDependencyService dependencyService;
        private readonly ILogger logger;

        public CreditCardRequestProcessor(ICreditScoreProvider creditScoreProvider, IRiskProvider riskProvider, IDependencyService dependencyService, ILogger logger)
        {
            this.creditScoreProvider = creditScoreProvider;
            this.riskProvider = riskProvider;
            this.dependencyService = dependencyService;
            this.logger = logger;
        }

        public AnalysisResponse Process(AnalysisRequest request)
        {
            var duration = Stopwatch.StartNew();
            //var decision = AnalysisResponse.Rejected;
            //var hasDecision = false;

            logger.LogDebug($"Iniciando processamento da requisição em {duration}");

            var score = creditScoreProvider.GetScoreByCpf(request?.CPF);
            var risk = riskProvider.GetRisk(request?.Idade);
            var isVip = dependencyService.Objeto1.Objeto2.IsVip;

            /*
            if (score <= 300)
                (decision, hasDecision) = (AnalysisResponse.Rejected, true);
            //{
            //    decision = AnalysisResponse.Rejected;
            //    hasDecision = true;
            //}

            if (!hasDecision && score > 950)
            {
                decision = AnalysisResponse.Approved;
                hasDecision = true;
            }
            
            if (!hasDecision && risk == Enums.CreditRisk.Low)
            {
                decision = AnalysisResponse.Approved;
                hasDecision = true;
            }

            if (!hasDecision && risk == Enums.CreditRisk.High)
            {
                decision = AnalysisResponse.Rejected;
                hasDecision = true;
            }

            if (!hasDecision && dependencyService.Objeto1.Objeto2.IsVip)
            {
                decision = AnalysisResponse.Approved;
                hasDecision = true;
            }
            */

            var decision = (score, risk, isVip) switch
            {
                (_, _, true) => AnalysisResponse.Approved, // VIP tem prioridade
                ( <= 300, _, _) => AnalysisResponse.Rejected,
                ( > 950, _, _) => AnalysisResponse.Approved,
                (_, Enums.CreditRisk.Low, _) => AnalysisResponse.Approved,
                (_, Enums.CreditRisk.High, _) => AnalysisResponse.Rejected,
                _ => AnalysisResponse.Rejected
            };

            duration.Stop();
            logger.LogDebug("Finalizado o processamento da requisição");

            return decision;
        }
    }
}
