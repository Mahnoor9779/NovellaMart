using System;
using System.Collections.Generic;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.BL.Model_Classes;

namespace NovellaMart.Core.BL.Model_Classes
{
    public class FlashSaleRuntimeData
    {
        public Dictionary<int, CircularQueue<CustomerRequestBL>> ProductQueues { get; set; } = new();
        public Dictionary<string, string> UserRequestStatus { get; set; } = new();
        public Dictionary<string, DateTime> AllocationExpiry { get; set; } = new();
        public HashSet<string> InCheckoutProcess { get; set; } = new();
        public List<string> ActivityLogs { get; set; } = new();
        public Dictionary<string, CustomerRequestBL> ActiveRequests { get; set; } = new(); 
        public int ActiveFlashSaleId { get; set; }
    }
}
