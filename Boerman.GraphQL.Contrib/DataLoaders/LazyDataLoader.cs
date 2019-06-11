using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Boerman.GraphQL.Contrib.DataLoaders
{
    public class LazyDataLoader : ILazyLoader
    {
        private bool _disposed;
        private IDictionary<string, bool> _loadedStates;
        protected virtual IDiagnosticsLogger<DbLoggerCategory.Infrastructure> Logger { get; }

        protected virtual DbContext Context { get; }

        public LazyDataLoader(DbContext context)
        {
            //var types = context.Model.GetEntityTypes();
            //var properties = types.Select(q => q.GetProperties());
            //types.Select(q => q.)
        }

        public void Load(object entity, [CallerMemberName] string navigationName = null)
        {
            //Check.NotNull(entity, nameof(entity));
            //Check.NotEmpty(navigationName, nameof(navigationName));

            if (ShouldLoad(entity, navigationName, out var entry))
            {
                entry.
                //entry.Load();
            }
        }

        public Task LoadAsync(object entity, CancellationToken cancellationToken = default, [CallerMemberName] string navigationName = null)
        {
            throw new NotImplementedException();
        }

        private bool ShouldLoad(object entity, string navigationName,
            out NavigationEntry navigationEntry)
        {
            if (_loadedStates != null
                && _loadedStates.TryGetValue(navigationName, out var loaded)
                && loaded)
            {
                navigationEntry = null;
                return false;
            }

            if (_disposed)
            {
                Logger.LazyLoadOnDisposedContextWarning(Context, entity, navigationName);
            }
            else if (Context.ChangeTracker.LazyLoadingEnabled)
            {
                var entityEntry = Context.Entry(entity); // Will use local-DetectChanges, if enabled.
                var tempNavigationEntry = entityEntry.Navigation(navigationName);

                if (entityEntry.State == EntityState.Detached)
                {
                    Logger.DetachedLazyLoadingWarning(Context, entity, navigationName);
                }
                else if (!tempNavigationEntry.IsLoaded)
                {
                    Logger.NavigationLazyLoading(Context, entity, navigationName);

                    navigationEntry = tempNavigationEntry;
                    return true;
                }
            }

            navigationEntry = null;
            return false;
        }
    }
}
