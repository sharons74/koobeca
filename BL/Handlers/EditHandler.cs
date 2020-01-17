using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    public class EditHandler : UpdateHandler
    {
        public EditHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

    }
}
