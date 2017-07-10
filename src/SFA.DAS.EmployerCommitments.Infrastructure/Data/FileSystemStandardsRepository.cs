﻿using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Data.Repositories;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Data
{
    public class FileSystemStandardsRepository : FileSystemRepository, IStandardsRepository
    {
        private const string fileName = "standards";

        public FileSystemStandardsRepository()
            : base("Standards")
        {
        }

        public async Task<Standard[]> GetAllAsync()
        {
            return await ReadFileById<Standard[]>(fileName);
        }

        public async Task<Standard> GetByCodeAsync(string code)
        {
            var standards = await ReadFileById<Standard[]>(fileName);

            return standards.SingleOrDefault(x => x.Id == code);
        }
    }
}