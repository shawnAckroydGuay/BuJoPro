namespace BuJoProApplicationLogic.BuJoCreator
{
    public interface IAgendaCreator
    {
        byte[] CreerLePlanificateurEnPdf(int premierMois, int nombreDeMoisVoulu = 6);
    }
}
