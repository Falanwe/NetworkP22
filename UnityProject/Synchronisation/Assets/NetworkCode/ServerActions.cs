using System.Collections;
using System.Collections.Generic;


namespace Synchronisation
{
    public enum  ServerAction : byte
    {
        SendClientId = 0,
        SendUpdate = 1
    }
}
