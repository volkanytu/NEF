using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.ConsoleApp.CreateCampaignActivityAndList
{
    public class ProcessResult
    {
        public ProcessResult(bool isSuccess, string result, string message)
        {
            this.IsSuccess = isSuccess;
            this.Result = result;
            this.Message = message;
        }

        public bool IsSuccess { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }
}
