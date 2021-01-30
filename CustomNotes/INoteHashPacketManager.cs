using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNotes.Interfaces
{
    internal interface INoteHashPacketManager
    {

        bool HasHashFromPlayer(IConnectedPlayer connectedPlayer);
        string GetHashFromPlayer(IConnectedPlayer connectedPlayer);

    }
}
