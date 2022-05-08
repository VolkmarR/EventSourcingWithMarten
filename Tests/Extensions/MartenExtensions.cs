using Marten.Services;
using Npgsql;

namespace Tests.Extensions;

internal static class MartenExtensions
{
    public static void AddSerilog(this StoreOptions options)
        => options.Logger(new SerilogMartenLogger());

    public class SerilogMartenLogger : IMartenLogger, IMartenSessionLogger
    {
        public IMartenSessionLogger StartSession(IQuerySession session)
        {
            return this;
        }

        public void SchemaChange(string sql)
        {
            Log.Debug("Executing DDL change:");
            Log.Debug(sql);
        }

        public void LogSuccess(NpgsqlCommand command)
        {
            Log.Debug(command.CommandText);
        }

        public void LogFailure(NpgsqlCommand command, Exception ex)
        {
            Log.Error(ex, command.CommandText);
        }

        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
        {
        }

        public void OnBeforeExecute(NpgsqlCommand command)
        {
        }
    }
}
