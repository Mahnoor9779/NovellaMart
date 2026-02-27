## 💎 NovellaMart: E-Commerce & Flash Sale Platform

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white) ![Blazor](https://img.shields.io/badge/blazor-%235C2D91.svg?style=for-the-badge&logo=blazor&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Bootstrap](https://img.shields.io/badge/bootstrap-%238511FA.svg?style=for-the-badge&logo=bootstrap&logoColor=white)

**NovellaMart** is a full-stack e-commerce web application engineered to handle high-concurrency traffic during flash sales.Built with **C# and ASP.NET Core Blazor**, it features hierarchical sub-category navigation, dynamic price-range filtering, and an advanced admin dashboard for real-time order and stock management. 

What sets NovellaMart apart is its underlying architecture: it utilizes custom Data Structures and Algorithms (DSA) to solve real-world system design challenges like the "thundering herd" problem during limited-time sales.

---

## ✨ Key Features

### 🛒 Customer Experience
* **Advanced Catalog & Search:** Hierarchical category and sub-category navigation.
* **Dynamic Filtering:** Filter products by specific price ranges and sort by price or newest arrivals.
* **Distance-Based Delivery:** Automated shipping calculations based on user location.
* **Smart Cart Management:** Add items, adjust quantities, and use a unique **"Undo"** feature to instantly restore accidentally deleted cart items.
* **Promo Codes:** Integrated discount application during checkout.

### ⚡ Flash Sale Engine
* **Fair Allocation:** Users joining a flash sale are placed in a waiting queue. Products are allocated strictly on a mathematically fair, first-come-first-served basis using timestamp processing.
* **Concurrency Control:** Prevents duplicate entries and overselling during high-traffic traffic bursts.

### 🛡️ Admin Dashboard
* **Real-Time Monitoring:** View live revenue, total orders, and active users.
* **Flash Sale Management:** Create new sales, set discounts, and monitor the live allocation logs (Allocated vs. Rejected).
* **Order Lifecycle:** Track global order history and manage shipping statuses.

---

## 🧠 Under the Hood: Data Structures & Algorithms
To ensure optimal performance and zero dependencies on heavy external databases, NovellaMart is powered entirely by custom-built data structures:

* **Min-Priority Heap:** Allocates limited flash sale stock by sorting users based on exact request timestamps (`O(log k)`).
* **Circular Queue:** Acts as a burst buffer to safely line up incoming flash sale requests without crashing the system.
* **AVL Trees:** Powers the product catalog, enabling extremely fast (`O(log n)`) price filtering and sorting across thousands of items.
* **Stacks (LIFO):** Tracks previous cart states to power the instant "Undo" functionality.
* **HashMaps (HashSets & Dictionaries):** Ensures `O(1)` lookups to prevent users from joining a flash sale twice or checking out with expired reservations.
* **Custom Doubly Linked Lists:** Manages dynamic datasets with frequent insertions/deletions, such as the global order history and live cart items.

Data persistence is handled via an optimized **JSON-based storage system** (`products.json`, `orders.json`, etc.).

---

## 🚀 Getting Started

### Prerequisites
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (or VS Code with C# Dev Kit)
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
### Installation
1. **Clone the repository:**
   ```bash
   git clone https://github.com/Mahnoor9779/NovellaMart
   cd NovellaMart
