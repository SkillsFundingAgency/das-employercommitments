namespace SFA.DAS.EmployerCommitments.Web.Validators.Messages
{
    public struct ValidationMessage
    {
        public ValidationMessage(string text, string errorCode)
        {
            ErrorCode = errorCode;
            Text = text;
        }

        public string Text { get; }

        public string ErrorCode { get; }

        public static string ExtractBannerMessage(string errorMessage)
        {
            var seperatorIndex = errorMessage.IndexOf("||");
            return seperatorIndex == -1 ? errorMessage : errorMessage.Substring(0, seperatorIndex);
        }

        public static string ExtractFieldMessage(string errorMessage)
        {
            var seperatorIndex = errorMessage.IndexOf("||");
            return seperatorIndex == -1 ? errorMessage : errorMessage.Substring(seperatorIndex + 2);
        }
    }
}