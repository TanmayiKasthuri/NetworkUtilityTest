using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Extensions;
using NetworkUtility.DNS;
using NetworkUtility.Ping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtility.Tests.PingTests
{
    public class NetworkServiceTests
    {
        private readonly NetworkService _pingService;
        private readonly IDns _dns;
        public NetworkServiceTests()
        {
            _dns = A.Fake<IDns>();//mocking _dns as you would not want to send original data
            _pingService =new NetworkService(_dns);//whatever dependencies are present in the function/class which we are testing, those should be bought in
        }
        [Fact]
        public void NetworkService_SendPing_ReturnString()
        {
            //Arrange
            //MockType
            A.CallTo(()=>_dns.sendDns()).Returns(true);//we declare value what _dns.sendDns would return and test with it
            //SUT
            //var pingService = new NetworkService();
            //Act
            var result=_pingService.SendPing();
            //Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be("Ping Sent!");
            result.Should().Contain("Sent", Exactly.Once());
        }
        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 2, 4)]
        public void NetworkService_PingTimeout_ReturnInt(int a, int b, int expected)
        {
            //Arrange
            //SUT-System Under Test-If you are newing up an object in every single test you put it up in constructor
            //var pingService=new NetworkService();
            //Act
            var result = _pingService.PingTimeout(a, b);
            //Assert
            result.Should().Be(expected);
            result.Should().BeGreaterThanOrEqualTo(2);
        }
        [Fact]
        public void NetworkService_LastPingDate_ReturnDate()
        {
            //Arrange
            //Act
            var result = _pingService.LastPingDate();
            //Assert
            result.Should().BeAfter(1.January(2010));
            result.Should().BeBefore(1.January(2050));
        }

        [Fact]
        public void NetworkService_GetPingOptions_ReturnObject()
        {
            //Arrange
            var expected = new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            };
            //Act
            var result=_pingService.GetPingOptions();

            //Assert
            //If you are comparing reference or objects instead of string/int you need to use BeEquivalentTo instead of be
            result.Should().BeOfType<PingOptions>();
            result.Should().BeEquivalentTo(expected);
            result.Ttl.Should().Be(expected.Ttl);
        }

        [Fact]
        public void NetworkService_MostRecentPings_ReturnObject()
        {
            //Arrange
            var expected = new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            };
            //Act
            var result = _pingService.MostRecentPings();

            //Assert
            //If you are comparing reference or objects instead of string/int you need to use BeEquivalentTo instead of be
            //result.Should().BeOfType<IEnumerable<PingOptions>>();
            result.Should().ContainEquivalentOf(expected);
            result.Should().Contain(x=>x.DontFragment==true);
        }
    }
}
