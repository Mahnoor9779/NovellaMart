using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class BinaryTreeNodeGeneric<type>
    {
        public type Data;
        public BinaryTreeNodeGeneric<type> Left;
        public BinaryTreeNodeGeneric<type> Right;

        public BinaryTreeNodeGeneric() { }

        public BinaryTreeNodeGeneric(type val)
        {
            Data = val;
            Left = null;
            Right = null;
        }
    }
}
