using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SR.AcceptanceTests.Client;
using SR.AcceptanceTests.Emulators;

namespace SR.AcceptanceTests.Requests
{
    public static class Shop
    {
        public static async Task CreateShop(this SrEmulator emulator, string name, string address, long businessId, CancellationToken token)
        {
            using var client = emulator.UsingClient;

            await client.CreateShopAsync(new CreateShopCommand
            {
                Name = name,
                Address = address,
                BusinessId = businessId
            }, token).ConfigureAwait(false);
        }

        public static async Task<Client.Shop> GetShop(this SrEmulator emulator, string name, CancellationToken token)
        {
            using var client = emulator.UsingClient;

            return await client.GetShopByNameAsync(name, token).ConfigureAwait(false);
        }

        public static async Task<ICollection<Client.Shop>> GetShopsByBusiness(this SrEmulator emulator, long businessId, CancellationToken token)
        {
            using var client = emulator.UsingClient;

            return await client.GetShopsQueryAsync(businessId, token).ConfigureAwait(false);
        }
    }
}