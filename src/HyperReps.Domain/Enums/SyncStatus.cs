using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Enums
{
    public enum SyncStatus
    {
        Idle,
        Queued,
        Syncing,
        Failed
    }
}
