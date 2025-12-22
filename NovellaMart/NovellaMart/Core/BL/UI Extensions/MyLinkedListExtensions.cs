namespace NovellaMart.Core.BL.UI_Extensions;
using System.Collections.Generic;
using NovellaMart.Core.BL.Data_Structures;

public static class MyLinkedListExtensions
{
    public static List<T> ToList<T>(this MyLinkedList<T> list)
    {
        List<T> result = new List<T>();

        var current = list.head;
        while (current != null)
        {
            result.Add(current.Data);
            current = current.Next;
        }

        return result;
    }
}

