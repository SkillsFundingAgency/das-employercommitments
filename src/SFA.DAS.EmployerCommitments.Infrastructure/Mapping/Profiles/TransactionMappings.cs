using AutoMapper;
using SFA.DAS.EmployerCommitments.Domain.Data.Entities.Transaction;
using SFA.DAS.EmployerCommitments.Domain.Models.Levy;
using SFA.DAS.EmployerCommitments.Domain.Models.Payments;
using SFA.DAS.EmployerCommitments.Domain.Models.Transaction;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Mapping.Profiles
{
    public class TransactionMappings : Profile
    {
        public TransactionMappings()
        {
            CreateMap<TransactionEntity, TransactionLine>();
            CreateMap<TransactionEntity, PaymentTransactionLine>();
            CreateMap<TransactionEntity, LevyDeclarationTransactionLine>();
        }
    }
}
