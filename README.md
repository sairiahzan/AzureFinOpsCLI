# Azure FinOps CLI 🚀

**Local-First Azure Cost and Resource Leak Detector**

Students, independent developers, and small startups often face surprise cloud bills at the end of the month due to forgotten test databases, public IPs, or virtual machines running on Azure. While budget alerts in the Azure Portal send emails, they can often be too late and do not proactively analyze or pinpoint the idle resources.

**Azure FinOps CLI** is a lightweight, local-first command-line tool that hooks into your existing local Azure CLI authentication. It scans your subscriptions for "zombie" resources (e.g., VMs running under 1% CPU utilization) and calculates exactly how much money they are wasting you per hour and per month.

## ✨ Features

- **🔒 Local-First Authentication:** It never asks for passwords or API keys. It safely uses your existing local `az login` session.
- **🧟 Zombie Resource Detection:** Finds virtual machines and databases running below a specific threshold (e.g., 1% CPU).
- **💸 Real-Time Pricing:** Integrated with the Azure Retail Prices API. It fetches accurate, up-to-date pricing based on the exact Region and SKU of your idle resources.
- **🌍 Multi-Currency Support:** View your wasted costs not just in US Dollars (USD), but in any supported currency like Turkish Lira (TRY) or Euros (EUR).
- **🛠️ Immediate Action:** Sleep (Deallocate) or delete zombie resources directly from your terminal.
- **🎨 Rich Terminal UI:** Beautiful, readable, and colorful tables powered by `Spectre.Console`.

## 🚀 Installation and Usage

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)

### Step 1: Login to Azure
Open your terminal and authenticate via Azure CLI:
```bash
az login
```

### Step 2: Run the Project
Navigate to the project directory and run the application:
```bash
cd AzureFinOpsCLI

# Basic scan (Defaults to <1% CPU threshold and USD currency)
dotnet run -- analyze

# Scan with custom CPU threshold, lookback days, and currency
dotnet run -- analyze --cpu-threshold 2.0 --lookback-days 14 --currency TRY
```

### Step 3: Take Action
If you want to stop or delete a resource found during the scan:
```bash
dotnet run -- action --id "<RESOURCE_ID_HERE>" --type sleep
```

## 🏗️ Technologies Used

- **C# / .NET 10**
- **System.CommandLine:** For modern command-line parsing.
- **Spectre.Console:** For rich terminal user interfaces and tables.
- **Azure Identity & Azure Resource Manager SDK:** For Azure authentication and resource management.
- **Azure Retail Prices API:** For real-time regional cost estimation.

## 🤝 Contributing
Contributions, issues, and feature requests are welcome! Feel free to check the issues page or submit a Pull Request (PR) to add zombie detection for new Azure resource types (Storage Accounts, App Services, etc.).

## 📄 License
This project is licensed under the **GNU General Public License v3.0 (GPL-3.0)** - see the [LICENSE](LICENSE) file for details.

---
*Built to protect developers from surprise cloud bills. ❤️*
