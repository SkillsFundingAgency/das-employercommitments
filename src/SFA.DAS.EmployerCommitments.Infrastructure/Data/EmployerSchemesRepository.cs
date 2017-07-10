using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Data.Repositories;
using SFA.DAS.EmployerCommitments.Domain.Models.PAYE;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Data
{
    public class EmployerSchemesRepository : BaseRepository, IEmployerSchemesRepository
    {
        public EmployerSchemesRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<PayeSchemes> GetSchemesByEmployerId(long employerId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", employerId, DbType.Int64);

                return await c.QueryAsync<PayeScheme>(
                    sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return new PayeSchemes
            {
                SchemesList = result.ToList()
            };
        }

        public async Task<PayeScheme> GetSchemeByRef(string empref)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@payeRef", empref, DbType.String);

                return await c.QueryAsync<PayeScheme>(
                    sql: "[employer_account].[GetPayeSchemesInUse]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();

        }
    }
}
