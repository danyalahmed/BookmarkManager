using System;

namespace CW03.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
            Title = "Error.";
            Message = "An error occurred while processing your request.";
        }

        public ErrorViewModel(string title, string message)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string RequestId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }        

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}