using CreditAnalysis.Interfaces;
using Moq;

namespace CreditAnalysis.Tests
{
    public class BaseTestClass
    {
        protected Mock<ICreditScoreProvider> creditScoreProviderMock;
        protected Mock<IRiskProvider> riskProviderMock;
        protected Mock<IDependencyService> dependencyService;
        protected Mock<ILogger> loggerMock;

        public const string CPF_FOR_LOW_SCORE = "123";
        public const string CPF_FOR_HIGH_SCORE = "456";

        public const int SCORE_LOW = 100;
        public const int SCORE_HIGH = 990;

        public BaseTestClass()
        {
            creditScoreProviderMock = new();
            riskProviderMock = new();
            dependencyService = new();
            loggerMock = new();

            creditScoreProviderMock.Setup(x => x.GetScoreByCpf(It.IsAny<string>())).Returns(400);
            riskProviderMock.Setup(x => x.GetRisk(It.IsAny<int?>())).Returns(Enums.CreditRisk.Medium);
            dependencyService.SetupProperty(x => x.Objeto1.Objeto2.IsVip, false);
            loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));
        }

        protected void SetupScore(string cpf, int score)
            => creditScoreProviderMock.Setup(x => x.GetScoreByCpf(cpf)).Returns(score);

        // Membros (Propriedades, Fields, Métodos) de classe -> Virtual, Abstract a gente consegue mocar, senão, não consegue (deep changes / Shimmers (eg: Prim e Microsoft Faker))

        //public class FakeCreditScoreProvider : ICreditScoreProvider
        //{
        //    private readonly int score;

        //    public FakeCreditScoreProvider(int score)
        //    {
        //        this.score = score;
        //    }
        //    public int GetScoreByCpf(string cpf)
        //        => score;
        //}
    }
}