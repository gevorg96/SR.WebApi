using System;

namespace SR.AcceptanceTests.Client
{
    public partial class SrClient: IDisposable
    {
        void IDisposable.Dispose() => _httpClient?.Dispose();
    }
}