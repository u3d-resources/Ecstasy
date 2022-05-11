


using System;
#if ENTITY_SHORT_ID
using IntID = System.UInt16; // optional ushort for IDs
#else
using IntID = System.Int32; // Int for direct indexing in DataPool without cast
#endif

namespace AV.ECS
{
    [Serializable]
    public struct Entity
    {
        public IntID id;

        // TODO: Need to actually implement versioning. Test and reproduce in which cases it is needed. 
        // The lack of it might cause fatal uncatchable bugs in a real production.
        //public uint version;

        // First implementation used type bitmask to check for attached components.
        // Now we simply check if sparse ID is assigned in a DataPool of required type.  
        //public BitMask32 componentsMask; 

        public static implicit operator IntID(Entity e) => e.id;
    }
}