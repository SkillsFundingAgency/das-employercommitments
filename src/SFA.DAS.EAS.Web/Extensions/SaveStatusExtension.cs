using SFA.DAS.EmployerCommitments.Web.Enums;

namespace SFA.DAS.EmployerCommitments.Web.Extensions
{
    public static class SaveStatusExtension
     {
         public static bool IsSend(this SaveStatus status)
         {
             return status == SaveStatus.ApproveAndSend || status == SaveStatus.AmendAndSend;
         }
 
         public static bool IsFinalApproval(this SaveStatus status)
         {
             return status == SaveStatus.Approve;
         }
     }
}