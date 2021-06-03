using SR.AcceptanceTests.Emulators;
using Xunit;

namespace SR.AcceptanceTests
{
    [CollectionDefinition(nameof(SrCollection))]
    public class SrCollection : ICollectionFixture<SrEmulator>
    { }
}