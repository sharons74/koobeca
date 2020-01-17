using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL
{
    public enum AuthorizationFlags : ushort
    {
        Nothing = 0,
        Like = 1,
        Comment = 2,
        Share = 4,
        View = 8,
        Delete = 16,
        Hide = 32,
        Edit = 64,
        DisableComments = 128,
        DisableShares = 256,
        All = 511
    }
}
