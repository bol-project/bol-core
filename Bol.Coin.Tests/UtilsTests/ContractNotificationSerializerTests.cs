using System.Collections.Generic;
using System.IO;
using Bol.Core.Helpers;
using Bol.Core.Model;
using FluentAssertions;
using Xunit;
using Parser = Bol.Core.Helpers.ContractNotificationSerializer.ContractNotificationParser;

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
        result.Message.Should().Be("False");

        //Account Assertions
        var account = result.Account;

        var expectedCommercialBalances = new Dictionary<string, string>
        {
            { "B4mZUZQF1T5Ddshn64cxuk9icE6FP5PqBW", "0" },
            { "BJnDMsb4viAPLCsXEUTb8D3cXbuQeXC2Cc", "0" },
            { "B8orwWRDjtTZ9evj84WkSk5okzA4EuZFKP", "0" },
            { "BQRZiuqeMd86yY2rrAjAhxXMM1LQhBbNnA", "0" },
            { "BBNiXghLioMtZmXBZUCM3uHJx6F3eMRLJz", "0" },
            { "BRZqVMSayhoMdZaHuQsqU9fJiXp6Buri3G", "0" },
            { "BSFxWTUz9RQfZvJBCmrYxyP1KVp8G6GZVZ", "0" },
            { "BNDfJu8zoBEL3WAwMRDGm1ctWgzjvoqx1X", "0" },
            { "B7gDdANPvZwmLtcy3Lif8QRKdCmNR14oPH", "0" },
            { "B9MvpN95Yi4MsN9Ne3GuEfWCSfzFyDV2Fp", "0" },
        };

        account.AccountStatus.Should().Be(AccountStatus.PendingCertifications);
        account.AccountType.Should().Be(AccountType.Birth);
        account.CodeName.Should().Be("P<GRC<PAPADOPOULOS<G<<<1963M<ca8FXTowBuE<1B941");
        account.Edi.Should().Be("b2b9730607eab74bdf74279c741addfbe6f2624341b5ce7c54a282daeda40848");
        account.MainAddress.Should().Be("BBBBfoqNsW21HvGkQUxCeUyd3HKEQXJSTg");
        account.BlockChainAddress.Should().Be("BDvVnvCnG9DJY5wGvi9huCi4qhwSGV2oND");
        account.SocialAddress.Should().Be("BA7nRCswUPtLUqYV2CcV1vF729TxtUFYGJ");
        account.ClaimBalance.Should().Be("100000000");
        account.TotalBalance.Should().Be("100000000");

        account.CommercialBalances.Should().BeEquivalentTo(expectedCommercialBalances);
        account.CommercialAddresses.Should().BeEquivalentTo(expectedCommercialBalances.Keys);

        account.Certifications.Should().Be(1);
        account.Certifiers.Should().ContainKey("P<GRC<TOKAS<T<<<1985M<gR2sdEhFT4lBCA");
        account.MandatoryCertifiers.Should().HaveCount(3);
        account.CertificationRequests.Should().HaveCount(1);
        account.LastCertificationHeight.Should().Be(12);
        account.IsCertifier.Should().Be(false);
        account.Collateral.Should().BeNull();
        account.CertificationFee.Should().BeNull();
        account.RegistrationHeight.Should().Be(10);
        account.LastClaimHeight.Should().Be(10);
        account.LastCertifierSelectionHeight.Should().Be(13);
        account.LastCertificationHeight.Should().Be(12);
        account.LastClaim.Should().Be("1000");
    }

    /// <summary>
    /// Courtesy of Chat GPT
    /// </summary>
    public class ParserTests
    {
        [Fact]
        public void TestSimpleValue()
        {
            var result = Parser.Parse("123");
            Assert.Equal("123", result);
        }

        [Fact]
        public void TestList()
        {
            var result = Parser.Parse("[1, 2, 3, 4, 5]");
            var list = (List<object>)result;
            Assert.Equal(5, list.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal((i + 1).ToString(), list[i]);
            }
        }

        [Fact]
        public void TestDictionary()
        {
            var result = Parser.Parse("{a:1,b:2,c:3}");
            var dict = (Dictionary<string, object>)result;
            Assert.Equal(3, dict.Count);
            Assert.Equal("1", dict["a"]);
            Assert.Equal("2", dict["b"]);
            Assert.Equal("3", dict["c"]);
        }

        [Fact]
        public void TestNestedData()
        {
            var text = "[1,{a:[2,3]},{b:{c:4}},5]";
            var result = Parser.Parse(text);
            var list = (List<object>)result;
            Assert.Equal(4, list.Count);
            Assert.Equal("1", list[0]);
            var dict1 = (Dictionary<string, object>)list[1];
            Assert.Single(dict1);
            var list1 = (List<object>)dict1["a"];
        }

        [Fact]
        public void TestNullValue()
        {
            var result = Parser.Parse("Null");
            Assert.Null(result);
        }

        [Fact]
        public void TestEmptyList()
        {
            var result = Parser.Parse("[]");
            var list = (List<object>)result;
            Assert.Equal("", list[0]);
        }

        [Fact]
        public void TestEmptyListParts()
        {
            var result = Parser.Parse("[,]");
            var list = (List<object>)result;
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void TestEmptyDictionary()
        {
            var result = Parser.Parse("{}");
            var dict = (Dictionary<string, object>)result;
            Assert.Empty(dict);
        }

        [Fact]
        public void TestBooleanValue()
        {
            var result = Parser.Parse("True");
            Assert.True((bool)result);
        }

        [Fact]
        public void TestMixedData()
        {
            var text = "{a:[1,{b:2}],c:{d:[3,4]},e:5}";
            var result = Parser.Parse(text);
            var dict = (Dictionary<string, object>)result;
            Assert.Equal(3, dict.Count);
            var list1 = (List<object>)dict["a"];
            Assert.Equal("1", list1[0]);
            var dict1 = (Dictionary<string, object>)list1[1];
            Assert.Equal("2", dict1["b"]);
            var dict2 = (Dictionary<string, object>)dict["c"];
            var list2 = (List<object>)dict2["d"];
            Assert.Equal("3", list2[0]);
            Assert.Equal("4", list2[1]);
            Assert.Equal("5", dict["e"]);
        }
    }
}
