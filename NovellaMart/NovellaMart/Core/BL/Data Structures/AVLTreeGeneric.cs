using System;
using System.Collections.Generic;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class AVLTreeGeneric<type>
    {
        public AVLNodeGeneric<type> Root;

        public AVLTreeGeneric()
        {
            Root = null;
        }

        public int GetHeight(AVLNodeGeneric<type> node)
        {
            if (node == null)
            {
                return 0;
            }
            return node.Height;
        }

        public int Max(int a, int b)
        {
            if (a > b)
            {
                return a;
            }
            return b;
        }

        public int GetBalance(AVLNodeGeneric<type> node)
        {
            if (node == null)
            {
                return 0;
            }
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        public AVLNodeGeneric<type> RightRotate(AVLNodeGeneric<type> y)
        {
            AVLNodeGeneric<type> x = y.Left;
            AVLNodeGeneric<type> t2 = x.Right;

            x.Right = y;
            y.Left = t2;

            y.Height = Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        public AVLNodeGeneric<type> LeftRotate(AVLNodeGeneric<type> x)
        {
            AVLNodeGeneric<type> y = x.Right;
            AVLNodeGeneric<type> t2 = y.Left;

            y.Left = x;
            x.Right = t2;

            x.Height = Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        public void Insert(type key, Func<type, type, int> compareFunction)
        {
            Root = InsertRecursive(Root, key, compareFunction);
        }

        public AVLNodeGeneric<type> InsertRecursive(AVLNodeGeneric<type> node, type key, Func<type, type, int> compareFunction)
        {
            if (node == null)
            {
                return new AVLNodeGeneric<type>(key);
            }

            int comparison = compareFunction(key, node.Data);

            if (comparison < 0)
            {
                node.Left = InsertRecursive(node.Left, key, compareFunction);
            }
            else if (comparison > 0)
            {
                node.Right = InsertRecursive(node.Right, key, compareFunction);
            }
            else
            {
                return node; // duplicates not allowed
            }

            node.Height = 1 + Max(GetHeight(node.Left), GetHeight(node.Right));

            int balance = GetBalance(node);

            // Left Left
            if (balance > 1 && compareFunction(key, node.Left.Data) < 0)
            {
                return RightRotate(node);
            }

            // Right Right
            if (balance < -1 && compareFunction(key, node.Right.Data) > 0)
            {
                return LeftRotate(node);
            }

            // Left Right
            if (balance > 1 && compareFunction(key, node.Left.Data) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left
            if (balance < -1 && compareFunction(key, node.Right.Data) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        public type Search(type key, Func<type, type, int> compareFunction)
        {
            AVLNodeGeneric<type> foundNode = SearchRecursive(Root, key, compareFunction);
            if (foundNode == null)
            {
                return default(type);
            }
            return foundNode.Data;
        }

        public AVLNodeGeneric<type> SearchRecursive(AVLNodeGeneric<type> node, type key, Func<type, type, int> compareFunction)
        {
            if (node == null)
            {
                return null;
            }

            int comparison = compareFunction(key, node.Data);

            if (comparison == 0)
            {
                return node;
            }

            if (comparison < 0)
            {
                return SearchRecursive(node.Left, key, compareFunction);
            }
            else
            {
                return SearchRecursive(node.Right, key, compareFunction);
            }
        }

        public type[] Inorder()
        {
            List<type> result = new List<type>();
            InorderHelper(Root, result);
            return result.ToArray();
        }

        public void InorderHelper(AVLNodeGeneric<type> node, List<type> result)
        {
            if (node == null)
            {
                return;
            }

            InorderHelper(node.Left, result);
            result.Add(node.Data);
            InorderHelper(node.Right, result);
        }

        public type[] Preorder()
        {
            List<type> result = new List<type>();
            PreorderHelper(Root, result);
            return result.ToArray();
        }

        public void PreorderHelper(AVLNodeGeneric<type> node, List<type> result)
        {
            if (node == null)
            {
                return;
            }

            result.Add(node.Data);
            PreorderHelper(node.Left, result);
            PreorderHelper(node.Right, result);
        }

        public type[] Postorder()
        {
            List<type> result = new List<type>();
            PostorderHelper(Root, result);
            return result.ToArray();
        }

        public void PostorderHelper(AVLNodeGeneric<type> node, List<type> result)
        {
            if (node == null)
            {
                return;
            }

            PostorderHelper(node.Left, result);
            PostorderHelper(node.Right, result);
            result.Add(node.Data);
        }
        public void GetRange(AVLNodeGeneric<type> node, type min, type max, List<type> result, Func<type, type, int> compare)
        {
            if (node == null) return;

            // 1. If current node is larger than min, go Left to search for smaller values
            if (compare(node.Data, min) > 0)
            {
                GetRange(node.Left, min, max, result, compare);
            }

            // 2. If current node is within the range, add it to result
            if (compare(node.Data, min) >= 0 && compare(node.Data, max) <= 0)
            {
                result.Add(node.Data);
            }

            // 3. If current node is smaller than max, go Right to search for larger values
            if (compare(node.Data, max) < 0)
            {
                GetRange(node.Right, min, max, result, compare);
            }
        }
    }
}