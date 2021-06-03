using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SR.AcceptanceTests.Client;
using SR.AcceptanceTests.Emulators;

namespace SR.AcceptanceTests.Requests
{
    public static class UoM
    {
        public static async Task CreateUoM(this SrEmulator emulator, string name, CancellationToken token)
        {
            using var client = emulator.UsingClient;

            await client.CreateUnitOfMeasureAsync(new CreateUnitOfMeasureCommand
            {
                Name = name
            }, token).ConfigureAwait(false);
        }
        
        public static async Task<UnitOfMeasure> GetUoMByName(this SrEmulator emulator, string name, CancellationToken token)
        {
            using var client = emulator.UsingClient;
            return await client.GetUnitOfMeasureByNameAsync(name, token).ConfigureAwait(false);
        }
        
        public static async Task<IReadOnlyCollection<UnitOfMeasure>> GetAllUoMs(this SrEmulator emulator, CancellationToken token)
        {
            using var client = emulator.UsingClient;
            var result = await client.GetUnitOfMeasuresAsync(token).ConfigureAwait(false);
            return result.ToList();
        }
    }
}