namespace CreditAnalysis.Interfaces
{
    public interface ICreditScoreProvider
    {
        int GetScoreByCpf(string cpf);
    }
}
