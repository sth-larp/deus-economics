using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Constants;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Logic;

namespace DeusCloud.Data
{
    public sealed class DeusData : DbContext
    {
        static DeusData()
        {
            //ContainerToToWindowsTableName = "ContainerToWindows";
            //PowerBarToTimersTableName = "PowerBarToTimers";
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountAccess> AccountAccesses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Constant> Constants { get; set; }
        public DbSet<Loyalty> Loyalties { get; set; }
        public DeusData() : this(null) { }
        public DeusData(string dbNameOrConnectionString)
            : base(string.IsNullOrEmpty(dbNameOrConnectionString) ? 
                  UserContext.DefaultConnectionStringName : dbNameOrConnectionString)
        {
            Database.SetInitializer(new DeusDBInitializer());
#if DEBUG
            Database.Log = (x => System.Diagnostics.Debug.WriteLine(x));
#endif
        }

        public void BeginFastSave()
        {
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public void CancelFastSave()
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
        }

        Exception UnwrapException(Exception exception)
        {
            Exception lastException = exception;
            while (lastException.InnerException != null)
                lastException = lastException.InnerException;

            if (lastException is SqlException)
            {
                var sqlException = (lastException as SqlException);
                if (sqlException.Number == 2627)
                    throw new DeusDuplicateException("Primary keys have not been managed appropriately across the topology", false);
            }

            return lastException;
        }

        public override int SaveChanges()
        {
            try
            {
                var count = base.SaveChanges();
                CancelFastSave();
                return count;
            }
            catch (Exception exception)
            {
                ExceptionDispatchInfo.Capture(UnwrapException(exception)).Throw();
                return 0;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var count = await base.SaveChangesAsync(cancellationToken);
                CancelFastSave();
                return count;

            }
            catch (Exception exception)
            {
                ExceptionDispatchInfo.Capture(UnwrapException(exception)).Throw();
                return 0;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }

}