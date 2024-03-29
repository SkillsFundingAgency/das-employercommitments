﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Infrastructure.ExecutionPolicies;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests
{
    public class NoopExecutionPolicy : ExecutionPolicy
    {
        public override void Execute(Action action)
        {
        }
        public override async Task ExecuteAsync(Func<Task> action)
        {
            await action();
        }

        public override T Execute<T>(Func<T> func)
        {
            return func();
        }
        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            return await func();
        }
    }
}
