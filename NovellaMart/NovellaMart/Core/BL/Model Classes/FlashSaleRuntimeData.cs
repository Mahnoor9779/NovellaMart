using System;
using System.Collections.Generic;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.BL.Model_Classes;

//Flash Sale Run Time Data
namespace NovellaMart.Core.BL.Model_Classes
{
    public class FlashSaleRuntimeData
    {
        public Dictionary<int, CircularQueue<CustomerRequestBL>> ProductQueues { get; set; } = new();
        public Dictionary<string, string> UserRequestStatus { get; set; } = new();
        public Dictionary<string, DateTime> AllocationExpiry { get; set; } = new();
        public HashSet<string> InCheckoutProcess { get; set; } = new();
        public List<string> ActivityLogs { get; set; } = new();
        // We also need ActiveRequests because UserRequestStatus keys (strings) map to these objects
        public Dictionary<string, CustomerRequestBL> ActiveRequests { get; set; } = new(); 
        public int ActiveFlashSaleId { get; set; } // Tracks which sale this data belongs to 
    }
}
