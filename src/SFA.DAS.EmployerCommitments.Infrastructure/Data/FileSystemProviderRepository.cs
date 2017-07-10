﻿using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Data.Repositories;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Data
{
    public class FileSystemProviderRepository : FileSystemRepository, IProviderRepository
    {
        private const string DataFileName = "provider_data";

        public FileSystemProviderRepository() 
            : base("Providers")
        {
        }

        public async Task<Providers> GetAllProviders()
        {
            return await ReadFileById<Providers>(DataFileName);
        }
    }
}