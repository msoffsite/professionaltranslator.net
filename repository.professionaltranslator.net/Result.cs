using System;
using System.Collections.Generic;

namespace Repository.ProfessionalTranslator.Net
{
    public class Result
    {
        public SaveStatus Status { get; set; }
        public List<string> Messages { get; set; }
        public Guid? ReturnId { get; set; }

        internal Result(SaveStatus status, List<string> messages, Guid? returnId)
        {
            Status = status;
            Messages = messages;
            ReturnId = status == SaveStatus.PartialSuccess || status == SaveStatus.Succeeded ? returnId : null;
        }

        public Result(SaveStatus status, string message, Guid? returnId)
        {
            Status = status;
            Messages = new List<string> { message };
            ReturnId = status == SaveStatus.PartialSuccess || status == SaveStatus.Succeeded ? returnId : null;
        }

        /// <summary>
        /// For use when return Id is unnecessary or on failed status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="messages"></param>
        internal Result(SaveStatus status, List<string> messages)
        {
            Status = status;
            Messages = messages;
            ReturnId = null;
        }

        /// <summary>
        /// For use when return Id is unnecessary or on failed status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        internal Result(SaveStatus status, string message)
        {
            Status = status;
            Messages = new List<string> { message };
            ReturnId = null;
        }

        public Result()
        {
            Status = SaveStatus.Undetermined;
            Messages = new List<string>();
            ReturnId = null;
        }
    }
}
