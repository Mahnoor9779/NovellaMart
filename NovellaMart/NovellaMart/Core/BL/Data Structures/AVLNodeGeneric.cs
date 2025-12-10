using System;

namespace NovellaMart
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
