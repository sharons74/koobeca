using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.APIs.Entities
{
    public class KBResult
    {
        public int status_code;
        public bool error;
        public string error_code;
        public string message;

        public override string ToString()
        {
            return $"status_code:{status_code} error_code:{error_code} message:{message}";
        }
    }
}
