using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    internal class LinkedListNode<type>
    {
        public type Data;
        public LinkedListNode<type> Next;

        public LinkedListNode(type value)
        {
            Data = value;
            Next = null;
        }
    }
}
