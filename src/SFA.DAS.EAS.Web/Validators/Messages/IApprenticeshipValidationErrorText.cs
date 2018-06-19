namespace SFA.DAS.EmployerCommitments.Web.Validators.Messages
{
    public interface IApprenticeshipValidationErrorText
    {
        ValidationMessage AcademicYearStartDate01 { get; }
        ValidationMessage DateOfBirth01 { get; }
        ValidationMessage DateOfBirth02 { get; }
        ValidationMessage DateOfBirth06 { get; }
        ValidationMessage FamilyName01 { get; }
        ValidationMessage FamilyName02 { get; }
        ValidationMessage GivenNames01 { get; }
        ValidationMessage GivenNames02 { get; }
        ValidationMessage LearnPlanEndDate01 { get; }
        ValidationMessage LearnPlanEndDate02 { get; }
        ValidationMessage LearnPlanEndDate03 { get; }
        ValidationMessage LearnPlanEndDateOverlap { get; }
        ValidationMessage LearnStartDate01 { get; }
        ValidationMessage LearnStartDate02 { get; }
        ValidationMessage LearnStartDate05 { get; }
        ValidationMessage LearnStartDateBeforeTransfersStart { get; }
        ValidationMessage LearnStartDateOverlap { get; }
        ValidationMessage EmployerRef01 { get; }
        ValidationMessage TrainingCode01 { get; }
        ValidationMessage TrainingPrice01 { get; }
        ValidationMessage TrainingPrice02 { get; }
        ValidationMessage Uln01 { get; }
        ValidationMessage Uln02 { get; }
    }
}