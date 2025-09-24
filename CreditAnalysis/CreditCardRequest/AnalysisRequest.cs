namespace CreditAnalysis.CreditCardRequest
{
    public class AnalysisRequest
    {
        public string? Nome { get; set; }
        public int? Idade { get; set; }
        public string? CPF { get; set; }
        public decimal? Renda { get; set; }

        public string ChamaMetodoProtected(int numero) 
            => MyProtected(numero);

        protected virtual string MyProtected(int numero)
        {
            return Random.Shared.Next(0, numero).ToString();
        }
    }
}
