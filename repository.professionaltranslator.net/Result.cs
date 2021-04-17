using System;
using System.Collections.Generic;

namespace Repository.ProfessionalTranslator.Net
{
    public class Result
    {
        public ResultStatus Status { get; set; }
        public List<string> Messages { get; set; }
        public Guid? ReturnId { get; set; }

        internal Result(ResultStatus status, List<string> messages, Guid? returnId)
        {
            Status = status;
            Messages = messages;
            ReturnId = status == ResultStatus.PartialSuccess || status == ResultStatus.Succeeded ? returnId : null;
        }

        public Result(ResultStatus status, string message, Guid? returnId)
        {
            Status = status;
            Messages = new List<string> { message };
            ReturnId = status == ResultStatus.PartialSuccess || status == ResultStatus.Succeeded ? returnId : null;
        }

        /// <summary>
        /// For use when return Id is unnecessary or on failed status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="messages"></param>
        internal Result(ResultStatus status, List<string> messages)
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
        internal Result(ResultStatus status, string message)
        {
            Status = status;
            Messages = new List<string> { message };
            ReturnId = null;
        }

        public Result()
        {
            Status = ResultStatus.Undetermined;
            Messages = new List<string>();
            ReturnId = null;
        }
    }
}
