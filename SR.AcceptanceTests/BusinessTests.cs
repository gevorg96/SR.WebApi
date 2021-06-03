using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SR.AcceptanceTests.Client;
using SR.AcceptanceTests.Emulators;
using SR.AcceptanceTests.Requests;
using Xunit;

namespace SR.AcceptanceTests
{
    [Collection(nameof(SrCollection))]
    public class BusinessTests
    {
        private readonly SrEmulator _client;
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10d);
        
        public BusinessTests(SrEmulator client) =>
            _client = client;

        [Fact]
        public async Task Create_Success()
        {
            using var lifetime = new CancellationTokenSource(Timeout);

            var businessName = Guid.NewGuid().ToString("N");
            var tel = "1234567890";

            await _client.CreateBusiness(businessName, tel, lifetime.Token).ConfigureAwait(false);
            var business = await _client.GetBusinessByName(businessName, lifetime.Token).ConfigureAwait(false);

            business.Should().NotBeNull();
            business.Id.Should().BeGreaterThan(0);
            business.Name.Should().Be(businessName);
            business.Telephone.Should().Be(tel);
        }

        [Fact]
        public async Task Get_Failed()
        {
            using var lifetime = new CancellationTokenSource(Timeout);
            var businessName = Guid.NewGuid().ToString("N");

            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
                await _client.GetBusinessByName(businessName, lifetime.Token).ConfigureAwait(false));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task CreateDuplicate_Failed()
        {
            using var lifetime = new CancellationTokenSource(Timeout);
            
            var businessName = Guid.NewGuid().ToString("N");
            var tel = "1234567890";

            await _client.CreateBusiness(businessName, tel, lifetime.Token).ConfigureAwait(false);
            
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>          
                    await _client.CreateBusiness(businessName, tel, lifetime.Token).ConfigureAwait(false));

            exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}