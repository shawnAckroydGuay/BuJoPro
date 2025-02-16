namespace BuJoProApplicationLogic.BuJoCreator
{
    public interface IAgendaCreator
    {
        byte[] CreerLePlanificateurEnPdf(int premierMois, string titre, byte[] imageCouverture, int nombreDeMoisVoulu = 6);
    }
}
