Markdown# 💎 NovellaMart: E-Commerce & Flash Sale Platform

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white) ![Blazor](https://img.shields.io/badge/blazor-%235C2D91.svg?style=for-the-badge&logo=blazor&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Bootstrap](https://img.shields.io/badge/bootstrap-%238511FA.svg?style=for-the-badge&logo=bootstrap&logoColor=white)

[cite_start]**NovellaMart** is a full-stack e-commerce web application engineered to handle high-concurrency traffic during flash sales[cite: 77]. [cite_start]Built with **C# and ASP.NET Core Blazor** [cite: 83, 84][cite_start], it features hierarchical sub-category navigation, dynamic price-range filtering, and an advanced admin dashboard for real-time order and stock management[cite: 93, 107]. 

[cite_start]What sets NovellaMart apart is its underlying architecture: it utilizes custom Data Structures and Algorithms (DSA) to solve real-world system design challenges like the "thundering herd" problem during limited-time sales[cite: 77, 81].

---

## ✨ Key Features

### 🛒 Customer Experience
* [cite_start]**Advanced Catalog & Search:** Hierarchical category and sub-category navigation[cite: 220, 223].
* [cite_start]**Dynamic Filtering:** Filter products by specific price ranges and sort by price or newest arrivals[cite: 216, 222, 223].
* **Distance-Based Delivery:** Automated shipping calculations based on user location.
* [cite_start]**Smart Cart Management:** Add items, adjust quantities, and use a unique **"Undo"** feature to instantly restore accidentally deleted cart items[cite: 165, 202].
* [cite_start]**Promo Codes:** Integrated discount application during checkout[cite: 208, 210].

### ⚡ Flash Sale Engine
* [cite_start]**Fair Allocation:** Users joining a flash sale are placed in a waiting queue[cite: 141]. [cite_start]Products are allocated strictly on a mathematically fair, first-come-first-served basis using timestamp processing[cite: 102, 149].
* [cite_start]**Concurrency Control:** Prevents duplicate entries and overselling during high-traffic traffic bursts[cite: 114, 118, 144].

### 🛡️ Admin Dashboard
* [cite_start]**Real-Time Monitoring:** View live revenue, total orders, and active users[cite: 107, 109].
* [cite_start]**Flash Sale Management:** Create new sales, set discounts, and monitor the live allocation logs (Allocated vs. Rejected)[cite: 108, 109].
* [cite_start]**Order Lifecycle:** Track global order history and manage shipping statuses[cite: 126, 206].

---

## 🧠 Under the Hood: Data Structures & Algorithms
[cite_start]To ensure optimal performance and zero dependencies on heavy external databases, NovellaMart is powered entirely by custom-built data structures[cite: 100, 101, 102, 103]:

* [cite_start]**Min-Priority Heap:** Allocates limited flash sale stock by sorting users based on exact request timestamps (`O(log k)`)[cite: 102, 149, 237].
* [cite_start]**Circular Queue:** Acts as a burst buffer to safely line up incoming flash sale requests without crashing the system[cite: 141, 144].
* [cite_start]**AVL Trees:** Powers the product catalog, enabling extremely fast (`O(log n)`) price filtering and sorting across thousands of items[cite: 103, 156, 159].
* [cite_start]**Stacks (LIFO):** Tracks previous cart states to power the instant "Undo" functionality[cite: 165, 167].
* [cite_start]**HashMaps (HashSets & Dictionaries):** Ensures `O(1)` lookups to prevent users from joining a flash sale twice or checking out with expired reservations[cite: 114, 118, 121].
* [cite_start]**Custom Doubly Linked Lists:** Manages dynamic datasets with frequent insertions/deletions, such as the global order history and live cart items[cite: 126, 127, 131, 134].

[cite_start]Data persistence is handled via an optimized **JSON-based storage system** (`products.json`, `orders.json`, etc.)[cite: 105].

---

## 🚀 Getting Started

### Prerequisites
* [cite_start][Visual Studio 2022](https://visualstudio.microsoft.com/) (or VS Code with C# Dev Kit) [cite: 85]
* [cite_start][.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) [cite: 86]

### Installation
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/Mahnoor9779/NovellaMart.git](https://github.com/Mahnoor9779/NovellaMart.git)
   cd NovellaMart
Open the Project:
Open the NovellaMart.sln file in Visual Studio 2022.Restore Dependencies:
Ensure NuGet packages are restored (specifically System.Text.Json).Data Initialization:
Ensure the Core/DL folder contains the necessary JSON files (users.json, orders.json, etc.).Run the Application:
Press F5 or click the Run button to start the interactive server.📸 Application GallerySearch & CatalogPrice Range Filtering<img src="images/search_dropdown.png" width="400" alt="Live Search"><img src="images/price_filter.png" width="400" alt="Price Filter">Flash Sale Waiting QueueCart Undo Feature<img src="images/waiting_queue.png" width="400" alt="Flash Sale Queue"><img src="images/cart_undo.png" width="400" alt="Undo Action">👨‍💻 Authors & CreditsMahnoor - Lead Developer / Architect * Developed as a Data Structures & Algorithms Lab Project at the Department of Computer Science, UET Lahore.
