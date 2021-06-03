using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SR.AcceptanceTests.Client;
using SR.AcceptanceTests.Emulators;

namespace SR.AcceptanceTests.Requests
{
    public static class Business
    {
        public static async Task CreateBusiness(this SrEmulator emulator, string name, string telephone, CancellationToken token)
        {
            using var client = emulator.UsingClient;
            await client.CreateBusinessAsync(name, telephone, new Client.Shop[0], token).ConfigureAwait(false);
        }

        public static async Task<Client.Business> GetBusinessByName(this SrEmulator emulator, string name, CancellationToken token)
        {
            using var client = emulator.UsingClient;
            return await client.GetBusinessQueryAsync(null, name, true, token).ConfigureAwait(false);
        }

        public static async Task<ICollection<Client.Business>> GetBusinesses(this SrEmulator emulator, CancellationToken token)
        {
            using var client = emulator.UsingClient;
            return await client.GetBusinessesAsync(true, token).ConfigureAwait(false);
        }

        public static async Task UpdateBusiness(this SrEmulator emulator, long id, string name, string telephone, CancellationToken token)
        {
            using var client = emulator.UsingClient;
            await client.UpdateBusinessAsync(id, new UpdateBusinessCommand
            {
                Id = id,
                Name = name,
                Telephone = telephone
            }, token).ConfigureAwait(false);
        }
    }
}