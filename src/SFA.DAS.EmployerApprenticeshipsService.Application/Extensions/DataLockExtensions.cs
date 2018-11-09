using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class DataLockExtensions
    {
        public static bool UnHandled(this DataLockStatus dl)
        {
            return !dl.IsResolved && dl.Status != Status.Pass;
        }

        public static bool IsPriceOnly(this DataLockStatus dataLockStatus)
        {
            return (int) dataLockStatus.ErrorCode == (int) DataLockErrorCode.Dlock07;
        }

        public static bool WithCourseError(this DataLockStatus dataLockStatus)
        {
            return dataLockStatus.ErrorCode.HasFlag(DataLockErrorCode.Dlock03)
                   || dataLockStatus.ErrorCode.HasFlag(DataLockErrorCode.Dlock04)
                   || dataLockStatus.ErrorCode.HasFlag(DataLockErrorCode.Dlock05)
                   || dataLockStatus.ErrorCode.HasFlag(DataLockErrorCode.Dlock06);
        }
    }
}
