using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace SR.AcceptanceTests.Emulators
{
    public abstract class Emulator : Disposable, IAsyncLifetime
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10d);
        private readonly Lazy<IHost> _host;

        protected Emulator()
        {
            _host = new Lazy<IHost>(CreateHost, LazyThreadSafetyMode.None);
        }

        protected IServiceProvider ServiceProvider => _host.Value.Services;

        protected abstract IHost CreateHost();

        async Task IAsyncLifetime.InitializeAsync()
        {
            using var cts = new CancellationTokenSource(Timeout);
            try
            {
                await _host.Value.StartAsync(cts.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException($"Cannot start {GetType().Name}.");
            }
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (!_host.IsValueCreated)
                return;

            using var cts = new CancellationTokenSource(Timeout);
            try
            {
                await _host.Value.StopAsync(cts.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException($"Cannot stop {GetType().Name}.");
            }
        }

        protected override void CleanUp()
        {
            if(_host.IsValueCreated)
                _host.Value.Dispose();
        }
    }
    
    public abstract class Disposable : IDisposable
    {
        private int _isDisposed;

        protected bool IsDisposed => Volatile.Read(ref this._isDisposed) > 0;

        protected abstract void CleanUp();

        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().AssemblyQualifiedName);
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) > 0)
                return;
            this.CleanUp();
        }
    }
}