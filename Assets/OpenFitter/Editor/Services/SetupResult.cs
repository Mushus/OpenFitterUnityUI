namespace OpenFitter.Editor.Services
{
    public sealed class SetupResult
    {
        public enum StatusType
        {
            Success,
            InProgress,
            Failed
        }

        public StatusType Status { get; set; }
        public string? Message { get; set; }

        public bool IsSuccess => Status == StatusType.Success;
        public bool IsInProgress => Status == StatusType.InProgress;
        public bool IsFailed => Status == StatusType.Failed;

        public static SetupResult Success() => new() { Status = StatusType.Success };
        public static SetupResult InProgress(string? message = null) => new()
        {
            Status = StatusType.InProgress,
            Message = message
        };
        public static SetupResult Failed(string message) => new()
        {
            Status = StatusType.Failed,
            Message = message
        };
    }
}
