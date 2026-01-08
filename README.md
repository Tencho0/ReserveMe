# ReserveMe

ReserveMe is a web application for managing venues and reservations.
It allows users to browse venues, make reservations, and leave reviews.

## 🚀 How to Start the Project

1. Open the solution file (`ReserveMe.sln`) in **Visual Studio**
2. Select the following startup projects:
   - `ReserveMe`
   - `ReserveMe.Server`
3. Run the application with: CTRL + F5

---

## ⚙️ How to Configure Startup Projects

1. Right-click on the **Solution** in Visual Studio
2. Select **Configure Startup Projects**
3. Choose **Multiple startup projects**
4. Set **Start** for:
- `ReserveMe.Server`
- `ReserveMe`
5. Click **OK**

---

## 📥 How to Clone the Repository

Clone the repository using:

`git clone https://github.com/Tencho0/ReserveMe.git`

---

## 🗄️ Database Setup
On the first application run, the system will automatically:
- Apply all pending database migrations
- Seed default venues
- Seed related users (venue owners and waiters)
- Seed a default administrator account

No manual database setup is required.


### 🔐 Default Administrator Credentials

- **Email:** superadmin@reserveme.com  
- **Password:** ReserveMe2@25  

---

## 🌿 Branches Strategy
- ### protected main - only the owner can merge into it
- ### develop - the branch from which we take version while developing
- ### naming conventions - feature/x-y
  - #### x - issue number
  - #### y - short describtion

**Example:**
feature/51-ImplementLivePage

---

## 🛠️ Tech Stack
- ASP.NET Core
- Entity Framework Core
- MSSQL
- Blazor WebAssembly

