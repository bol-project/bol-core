using System.IO;
using Bol.Coin.Tests.Utils;
using Bol.Core.Model;
using FluentAssertions;
using Xunit;

namespace Bol.Coin.Tests.UtilsTests;

public class ContractNotificationSerializerTests
{
    [Fact]
    public void Given_ContractNotification_Serializer_ShouldSerializeCorrectly()
    {
        var notification = File.ReadAllText("contractNotificationExample.txt");
        
        var result = ContractNotificationSerializer.Deserialize(notification);
        
        result.Operation.Should().Be("register");
        result.StatusCode.Should().Be(200);
        result.IsComplete.Should().Be(false);
        
        //Account Assertions
        var account = result.Account;
        
        account.AccountStatus.Should().Be(AccountStatus.PendingCertifications);
        account.AccountType.Should().Be(AccountType.Birth);
        account.CodeName.Should().Be("P<GRC<PAPPAS<S<MANU<CHAO<1983MP<LsDDs8n8snS5BCA");
        account.Edi.Should().Be("E3274F6BBD018F920E7D629BA035D211E68E41F23F28F5A9F9B89B7B0EA860DB");
        account.MainAddress.Should().Be("ANVyn3x4GAEypqaU5R2BS9hYeyS7oFJmi6");
        account.BlockChainAddress.Should().Be("APjUt8YCs1wydpxBL3XxC6oCFRsWb9TyCs");
        account.SocialAddress.Should().Be("ATyxNR4qQSGDwToRQQCkgeAuSBRsfaphXD");
        account.CommercialAddresses.Should().BeNull(); // TODO implement this field at Serializer
        account.ClaimBalance.Should().Be("100000000");
        account.TotalBalance.Should().Be("100000000");
        account.CommercialBalances.Should().BeNull(); 
        account.Certifications.Should().Be(0);
        account.Certifiers.Should().BeNull();
        account.MandatoryCertifier.Should().Be("P<GRC<TOKAS<T<<<1985M<gR2sdEhFT4lBCA");
        account.IsCertifier.Should().Be(false);
        account.Collateral.Should().BeNull();
        account.RegistrationHeight.Should().Be(101);
        account.LastClaimHeight.Should().Be(101);
    }
}
