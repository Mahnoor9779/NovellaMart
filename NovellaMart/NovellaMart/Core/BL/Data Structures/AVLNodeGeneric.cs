using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class AVLNodeGeneric<type>
    {
        public type Data;
        public AVLNodeGeneric<type> Left;
        public AVLNodeGeneric<type> Right;
        public int Height;

        public AVLNodeGeneric(type val)
        {
            Data = val;
            Left = null;
            Right = null;
            Height = 1;
        }
    }
}
