using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SR.AcceptanceTests.Emulators;
using SR.AcceptanceTests.Requests;
using Xunit;

namespace SR.AcceptanceTests
{
    [Collection(nameof(SrCollection))]
    public class ShopTests
    {
        private readonly SrEmulator _client;
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10d);
        
        public ShopTests(SrEmulator client) =>
            _client = client;

        [Fact]
        public async Task Create_Success()
        {
            using var lifetime = new CancellationTokenSource(Timeout);
            
            var businessName = Guid.NewGuid().ToString("N");
            var shopName = Guid.NewGuid().ToString("N");
            
            await _client.CreateBusiness(businessName, "123", lifetime.Token).ConfigureAwait(false);
            var business = await _client.GetBusinessByName(businessName, lifetime.Token).ConfigureAwait(false);
            
            await _client.CreateShop(shopName, "address", business.Id, lifetime.Token).ConfigureAwait(false);
            var shop = await _client.GetShop(shopName, lifetime.Token).ConfigureAwait(false);

            shop.Should().NotBeNull();
            business.Should().NotBeNull();
            shop.BusinessId.Should().Be(business.Id);
            shop.BusinessId.Should().BeGreaterThan(0);
            shop.Name.Should().Be(shopName);
        }
    }
}