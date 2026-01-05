using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class MyLinkedListNode<type>
    {
        public type Data;
        public MyLinkedListNode<type> Next;

        public MyLinkedListNode()
        {
            Data = default(type);
            Next = null;
        }

        public MyLinkedListNode(type value)
        {
            Data = value;
            Next = null;
        }
    }
}
