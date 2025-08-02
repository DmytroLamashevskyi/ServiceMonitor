# ServiceMonitor

**ServiceMonitor** is a lightweight and user-friendly utility for monitoring the availability and status of web services and servers.  
It supports checking services via HTTP health check APIs and ICMP ping, runs in the system tray, and displays notifications on status changes.

---

## Key Features

- Service monitoring with configurable check intervals  
- Support for HTTP Health Check API and Ping checks  
- Notifications for service status changes (online, offline, warning)  
- Automatic saving and loading of service lists  
- Runs in the system tray with a convenient context menu  
- Minimalistic WPF interface using MahApps.Metro  

---

## Technical Details

- Built with C# using .NET 9 and WPF  
- Architecture follows MVVM pattern with clear separation of Core, ViewModels, and Views  
- Dependency injection through constructor parameters  
- Monitoring logic implemented in `ServiceCheckerEngine` with async tasks for status checks  
- Data storage via `ServiceRepository` and `FileStorageService` using JSON serialization to user app data folder  
- Notifications implemented as custom toast windows with smooth animations and queue management  
- CI/CD pipeline using GitHub Actions for automated build, test, and GitHub Release publishing  

---

## Requirements

- Windows 10 or 11  
- .NET 9 Runtime (self-contained in published build)  

---

## Build and Run

1. Clone the repository:  
git clone https://github.com/DmytroLamashevskyi/ServiceMonitor.git
2. Open the solution in Visual Studio 2022 or later (with .NET 9 support)  
3. Build the project in Release configuration  
4. Run the generated `.exe` from the `publish` folder  

---

## Contact and Support

- Repository: [https://github.com/DmytroLamashevskyi/ServiceMonitor](https://github.com/DmytroLamashevskyi/ServiceMonitor)  
- Report issues or feature requests via GitHub Issues  

---

## License

MIT License © 2025 Dmytro Lamashevskyi

