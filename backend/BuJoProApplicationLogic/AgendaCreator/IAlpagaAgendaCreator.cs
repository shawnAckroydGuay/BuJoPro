namespace BuJoProApplicationLogic.BuJoCreator
{
    public interface IAlpagaAgendaCreator
    {
        byte[] CreerLePlanificateurEnPdf(int premierMois, string titre, byte[] imageCouverture, int nombreDeMoisVoulu = 6);
    }
}
