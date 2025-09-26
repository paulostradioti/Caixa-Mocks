using Moq;

namespace CreditAnalysis.Tests
{

    public class Contador
    {
        private int counter = 0;

        public virtual int GetCounter()
         => counter++;
    }

    public class ContadorShould
    {
        [Fact]
        public void SequenceMocking()
        {
            var mock = new Mock<Contador>();

            mock.SetupSequence(x => x.GetCounter())
                .Returns(1)
                .Returns(2)
                .Returns(3)
                .Throws(new Exception("Estouro do Contador"));

            Assert.Equal(1, mock.Object.GetCounter());
            Assert.Equal(2, mock.Object.GetCounter());
            Assert.Equal(3, mock.Object.GetCounter());

            var action = () => mock.Object.GetCounter();
            Assert.Throws<Exception>(() => action());
        }


        [Fact]
        public void MockWithMethodReturn()
        {
            int contador = 0;
            var mock = new Mock<Contador>();
            mock.Setup(x => x.GetCounter()).Returns(() => CalculaRetorno());

            Assert.Equal(0, mock.Object.GetCounter());
            Assert.Equal(1, mock.Object.GetCounter());
            Assert.Equal(2, mock.Object.GetCounter());

            var action = () => mock.Object.GetCounter();
            Assert.Throws<Exception>(() => action());

            int CalculaRetorno()
            {
                if (contador >= 3)
                    throw new Exception("Estouro");

                return contador++;
            }
        }


        // Fronteiras <- estado
        // Branca <- comportamento
    }
}
