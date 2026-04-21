# 🚀 SACCO Management System  
### Enterprise Financial Cooperative Platform (.NET Core | SQL Server)

## 📌 Overview
This is a full-featured **SACCO (Savings and Credit Cooperative Organization) Management System** built using **ASP.NET Core MVC and SQL Server**.  

The system is designed to handle **end-to-end financial operations**, including member management, loan processing, contributions, accounting, and reporting — similar to real-world fintech and microfinance platforms.

---

## 🎯 Key Features

### 👥 Member Management
- Member registration and approval workflow  
- Dynamic member profiling (individual / group / corporate)  
- KYC data capture (ID, KRA PIN, contacts)

### 💰 Loan Management
- Loan application, appraisal, approval, and disbursement  
- Multiple repayment methods:
  - AMRT (Amortized)
  - RBAL (Reducing Balance)
  - STL (Straight Line)  
- Loan repayment tracking and balance updates  
- Automated interest and penalty calculations  

### 📊 Contributions & Shares
- Savings and contributions tracking  
- Share capital management  
- Enforcement of minimum/maximum shareholding rules  

### 🧾 Accounting & GL Integration
- Double-entry accounting (GL Transactions)  
- Automated posting for:
  - Loan repayments  
  - Contributions  
  - Fees  
- Chart of Accounts configuration  
- Real-time balance updates  

### 📈 Reporting
- Member registration reports  
- Loan issuance and repayment reports  
- Financial summaries and statements  
- Integrated with **FastReport** for structured reporting  

---

## 🧠 System Capabilities
- Handles **transaction-heavy financial workflows**  
- Ensures **data consistency and integrity** across modules  
- Supports **multi-stage approval processes**  
- Designed for **scalable SACCO operations**

---

## 🛠️ Tech Stack

- **Backend:** C# (.NET Core / ASP.NET MVC)  
- **Database:** SQL Server  
- **ORM:** Entity Framework  
- **Reporting:** FastReport  
- **Frontend:** HTML, CSS, JavaScript, jQuery  

---

## 🏗️ Architecture Overview

The system follows a structured MVC-based architecture:

Controllers → Business Logic → Data Access → SQL Server

- Controllers handle HTTP requests and responses  
- Business logic manages financial rules and workflows  
- Data layer interacts with the database  
- Modular design for accounting, loans, and membership  

---

## 🔐 Key Engineering Highlights

- ✔ Complex loan repayment logic implementation  
- ✔ Financial transaction integrity using GL accounting principles  
- ✔ Multi-module system integration (Loans, Members, Accounting)  
- ✔ Real-time balance and transaction updates  
- ✔ Clean separation of concerns  

---

## 📦 Example Modules

- Members Registration  
- Loan Applications  
- Loan Repayments  
- Share Transactions  
- Accounting (GL Setup & Posting)  
- Reports  

---

## 🚀 Getting Started

### Prerequisites
- .NET SDK  
- SQL Server  
- Visual Studio  

### Setup

1. Clone the repository:
```bash
git clone https://github.com/biikip/SACCO-System.git

Open the solution in Visual Studio
Configure database connection in:
appsettings.json
Run database migrations or restore DB
Run the application:
dotnet run
📌 Use Case

This system is suitable for:

SACCOs / Credit Unions
Microfinance Institutions
Financial Cooperatives
Loan Management Platforms
📈 Future Improvements
API layer (RESTful services)
JWT Authentication & Authorization
Docker containerization
Cloud deployment (Azure / AWS)
Mobile integration (USSD / App)
👨‍💻 Author

Developed by Bii Kip

.NET Developer | Financial Systems Specialist
Experience in SACCO, loan processing, and accounting systems
📬 Contact  

For collaboration or opportunities, feel free to reach out.

biikip.kip@gmail.com
