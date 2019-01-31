namespace Bol.Coin.Abstractions
{
    public interface IBolRepository
    {
        void AddBols(byte[] address, double amount);
        void AddBols(double amount);
        void AddCertification(byte[] address, byte[] certifier);
        void AddRegisteredCompany();
        void AddRegisteredPerson();
        void BindCollateral(byte[] address);
        double GetBols(byte[] address);
        double GetBols();
        int GetCertifications(byte[] address);
        byte[][] GetCertifiers(byte[] address);
        byte[] GetCodeName(byte[] address);
        byte[] GetEdi(byte[] address);
        uint GetLastClaimHeight(byte[] address);
        uint GetRegistrationHeight(byte[] address);
        long GetTotalRegisteredCompanies();
        long GetTotalRegisteredPersons();
        bool IsCertifier(byte[] address);
        void RegisterAsCertifier(byte[] address);
        void ReleaseCollateral(byte[] address);
        void RemoveBols(byte[] address, double amount);
        void RemoveBols(double amount);
        void RemoveCertification(byte[] address, byte[] certifier);
        void SetBols(byte[] address, double amount);
        void SetBols(double amount);
        void SetCodeName(byte[] address, byte[] codeName);
        void SetEdi(byte[] address, byte[] edi);
        void SetLastClaimHeight(byte[] address, uint height);
        void SetRegistrationHeight(byte[] address, uint height);
        void UnregisterAsCertifier(byte[] address);
    }
}