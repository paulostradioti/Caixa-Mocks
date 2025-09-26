using CreditAnalysis.CreditCardRequest;
using Moq;
using Moq.Protected;

namespace CreditAnalysis.Tests
{
    public class CreditCardRequestProcessorShould : BaseTestClass
    {
        CreditCardRequestProcessor sut;

        public CreditCardRequestProcessorShould()
        {
            sut = new(creditScoreProviderMock.Object, riskProviderMock.Object, dependencyService.Object, loggerMock.Object);
        }

        [Fact]
        public void ReturnRejectedByDefault()
        {
            //Act
            var response = sut.Process(default);

            // Assert
            Assert.Equal(AnalysisResponse.Rejected, response);

            loggerMock.Verify(x => x.LogDebug(It.IsAny<string>()), Times.Exactly(2));
            loggerMock.Verify(x => x.LogDebug(It.IsAny<string>()), Times.AtLeastOnce);
            loggerMock.Verify(x => x.LogDebug(It.IsAny<string>()), Times.AtLeast(2));
            loggerMock.Verify(x => x.LogDebug(It.IsAny<string>()), Times.AtMost(2));
        }

        [Fact]
        public void AutoRejectLowCreditScores()
        {
            SetupScore(CPF_FOR_LOW_SCORE, SCORE_LOW);

            //Act
            var response = sut.Process(new AnalysisRequest { CPF = CPF_FOR_LOW_SCORE });

            // Assert
            Assert.Equal(AnalysisResponse.Rejected, response);
        }

        [Fact]
        public void AutoApproveHightCreditScores()
        {
            SetupScore(CPF_FOR_HIGH_SCORE, SCORE_HIGH);

            //Act 
            var response = sut.Process(new AnalysisRequest { CPF = CPF_FOR_HIGH_SCORE });

            // Assert
            Assert.Equal(AnalysisResponse.Approved, response);
        }

        [Fact]
        public void ApproveOrRejectBasedOnCpfStart()
        {
            creditScoreProviderMock.Setup(x => x.GetScoreByCpf(It.Is<string>(x => x.StartsWith("9")))).Returns(990);
            creditScoreProviderMock.Setup(x => x.GetScoreByCpf(It.Is<string>(x => x.StartsWith("1")))).Returns(110);

            // Arrange
            var requestApprove = new AnalysisRequest() { CPF = "987654321" };
            var requestReject = new AnalysisRequest() { CPF = "123456789" };

            //Act
            var responseApprove = sut.Process(requestApprove);
            var responseReject = sut.Process(requestReject);

            // Assert
            Assert.Equal(AnalysisResponse.Approved, responseApprove);
            Assert.Equal(AnalysisResponse.Rejected, responseReject);
        }

        [Theory]
        [InlineData(30, AnalysisResponse.Approved)]
        [InlineData(60, AnalysisResponse.Rejected)]
        public void AutoRespondBasedOnRisk(int idade, AnalysisResponse expected)
        {
            riskProviderMock.Setup(x => x.GetRisk(It.IsInRange(30, 50, Moq.Range.Inclusive))).Returns(Enums.CreditRisk.Low);
            riskProviderMock.Setup(x => x.GetRisk(It.IsInRange(51, int.MaxValue, Moq.Range.Inclusive))).Returns(Enums.CreditRisk.High);

            //Act 
            var request = new AnalysisRequest { Idade = idade };
            var response = sut.Process(request);

            // Assert
            Assert.Equal(expected, response);
        }

        [Theory]
        [InlineData(30, AnalysisResponse.Approved)]
        [InlineData(40, AnalysisResponse.Approved)]
        [InlineData(50, AnalysisResponse.Approved)]
        [InlineData(60, AnalysisResponse.Rejected)]
        [InlineData(10, AnalysisResponse.Rejected)]
        public void AutoRespondBasedOnAge(int idade, AnalysisResponse expected)
        {
            riskProviderMock.Setup(x => x.GetRisk(It.IsAny<int>())).Returns(Enums.CreditRisk.High);
            riskProviderMock.Setup(x => x.GetRisk(It.IsIn(30, 40, 50))).Returns(Enums.CreditRisk.Low);

            //Act 
            var request = new AnalysisRequest { Idade = idade };
            var response = sut.Process(request);

            // Assert
            Assert.Equal(expected, response);
        }

        [Theory]
        [InlineData("0123456789", AnalysisResponse.Approved)]
        [InlineData("abcdefg", AnalysisResponse.Rejected)]
        public void ApproveWhenRegexIsValid(string cpf, AnalysisResponse expected)
        {
            creditScoreProviderMock.Setup(x => x.GetScoreByCpf(It.IsAny<string>())).Returns(0);
            creditScoreProviderMock.Setup(x => x.GetScoreByCpf(It.IsRegex(@"^[0-9]+$"))).Returns(1000);

            var request = new AnalysisRequest { CPF = cpf };
            var response = sut.Process(request);

            Assert.Equal(expected, response);
        }

        [Fact]
        public void ShowCapturedParameters()
        {
            var listaCpfs = new List<string>();
            creditScoreProviderMock.Setup(x => x.GetScoreByCpf(Capture.In(listaCpfs)));

            var request1 = new AnalysisRequest { CPF = "123456789" };
            var request2 = new AnalysisRequest { CPF = "987654321" };

            sut.Process(request1);
            sut.Process(request2);

            Assert.True(listaCpfs.Count == 2);
        }

        [Fact]
        public void MockPropertyCallChain()
        {
            dependencyService.SetupProperty(x => x.Objeto1.Objeto2.IsVip, true);
            //userServiceMock.SetupAllProperties();

            var response = sut.Process(default);

            Assert.Equal(AnalysisResponse.Approved, response);

            dependencyService.VerifyGet(x => x.Objeto1.Objeto2.IsVip, Times.Once);
            dependencyService.VerifySet(x => x.Objeto1.Objeto2.IsVip = false, Times.Never);
        }

        [Fact]
        public void MockProtected()
        {
            // Interfaces
            // Métodos/Propriedades Abstratas e Virtual

            //var instancia = new AnalysisRequest();
            //var resultado = instancia.ChamaMetodoProtected(100);

            var analysisRequestMock = new Mock<AnalysisRequest>();
            analysisRequestMock.Protected().Setup<string>("MyProtected", ItExpr.IsAny<int>()).Returns("My Protected Method Mocked!");

            var result = analysisRequestMock.Object.ChamaMetodoProtected(100);

            Assert.True(result.Length > 0);
        }
    }
}