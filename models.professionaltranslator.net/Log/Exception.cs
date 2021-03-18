using System;

namespace Models.ProfessionalTranslator.Net.Log
{
    public class Exception : Base
    {
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        public string Type { get; set; }
        public string Class { get; set; }
        public DateTime? DateCreated { get; set; }

        public Exception() {}

        public Exception(System.Exception inputItem, string className)
        {
            Id = Guid.Empty;
            Message = inputItem.Message;
            Stacktrace = inputItem.StackTrace;
            Type = inputItem.GetType().Name;
            Class = className;
            DateCreated = DateTime.Now;
        }
    }
}
