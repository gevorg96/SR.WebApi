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
    public class UnitOfMeasureTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10d);

        private readonly SrEmulator _client;
        
        public UnitOfMeasureTests(SrEmulator client) =>
            _client = client;

        [Fact]
        public async Task Create_Success()
        {
            using var lifetime = new CancellationTokenSource(Timeout);

            var name = Guid.NewGuid().ToString("N");
            await _client.CreateUoM(name, lifetime.Token).ConfigureAwait(false);

            var uom = await _client.GetUoMByName(name, lifetime.Token).ConfigureAwait(false);
            uom.Should().NotBeNull();
            uom.Id.Should().BeGreaterThan(0);
            uom.Name.Should().Be(name);
        }

        [Fact]
        public async Task CreateDuplicate_Fail()
        {
            using var lifetime = new CancellationTokenSource(Timeout);

            var name = Guid.NewGuid().ToString("N");
            await _client.CreateUoM(name, lifetime.Token).ConfigureAwait(false);
            var exception = await Assert.ThrowsAsync<ApiException<ProblemDetails>>(async () =>
                await _client.CreateUoM(name, lifetime.Token).ConfigureAwait(false));
            
            exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Get_Failed()
        {
            using var lifetime = new CancellationTokenSource(Timeout);
            
            var name = Guid.NewGuid().ToString("N");

            var exception = await Assert.ThrowsAsync<ApiException<ProblemDetails>>(async () =>
                await _client.GetUoMByName(name, lifetime.Token).ConfigureAwait(false));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}