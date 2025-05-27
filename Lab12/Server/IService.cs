using System.ServiceModel;

namespace Server
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void Koduj(string nazwa, string tresc);

        [OperationContract]
        string Pobierz(string nazwa);
    }
}
