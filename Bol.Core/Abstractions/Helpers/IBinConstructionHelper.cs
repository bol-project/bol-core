namespace Bol.Core.Abstractions.Helpers
{
    internal interface IBinConstructionHelper
    {
        string Construct(char gender, int dayOfBirth, string firstName, string surName, string postalCode);
    }
}
