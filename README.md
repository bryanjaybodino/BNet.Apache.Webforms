# 🚀 ASP.NET WebForms Standalone Hosting Server

A lightweight, standalone ASP.NET WebForms host and web server built in C# using `HttpListener` and `ApplicationHost.CreateApplicationHost`. It allows you to host ASP.NET WebForms (`.aspx`), static files, and process HTTP requests without requiring full IIS or IIS Express installed.

---

## ✨ Features

- ⚡ **Lightweight ASP.NET Hosting:** Serves `.aspx` pages dynamically using a custom `SimpleWorkerRequest` implementation.
- 📁 **Static File Handling:** Direct, high-performance streaming for `.css`, `.js`, `.png`, `.jpg`, `.json`, `.svg`, and other static assets.
- 🔒 **Automatic Firewall & Port Management:** Integrated `PortManager` automatically adds `netsh http urlacl` reservations and opens Windows Firewall ports.
- 🛡️ **Auto-Elevation (Admin Mode):** Checks for Administrator privileges on startup to configure netsh rules automatically.
- 🔄 **Self-Healing & Restart Capability:** Global exception handling and unhandled exception recovery auto-restarts the server upon unexpected failures or AppDomain unloads.
- 🚀 **Windows Startup Support:** Option to auto-register into Windows CurrentVersion Run keys for silent startup.
- 🖥️ **Windows Forms & Console Compatible:** Easily usable in both Console applications and background worker services inside WinForms apps.

---

## 📂 Deployment & File Placement

To ensure the server correctly locates your physical web files and compiled binaries (`.aspx`, `.dll`, etc.), **place the compiled executable (`.exe`) and its configuration file inside the `bin` folder** of your project directory structure:

```text
YourProjectRoot/
├── Default.aspx
├── About.aspx
├── web.config
└── bin/
    ├── BNet.Apache.Webforms.exe         <-- Place the executable here
    └── BNet.Apache.Webforms.exe.config  <-- Place the config file here
```

---

## ⚙️ Configuration

The server is configured through the `appSettings` section of `BNet.Apache.Webforms.exe.config`, which must sit next to the executable in the `bin` folder.

### Example `BNet.Apache.Webforms.exe.config`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <!-- The port number the web server will listen on -->
    <add key="Port" value="8180" />

    <!-- The default startup page -->
    <add key="DefaultPage" value="Default.aspx" />

    <!-- The virtual directory path (use "/" for root) -->
    <add key="VirtualDir" value="/" />
  </appSettings>
</configuration>
```

### Settings Reference

| Key           | Example Value  | Description                                                                 |
|---------------|----------------|-----------------------------------------------------------------------------|
| `Port`        | `8180`         | The TCP port the web server listens on.                                     |
| `DefaultPage` | `Default.aspx` | The page served when a request targets the site root (e.g. `/`).            |
| `VirtualDir`  | `/`            | The virtual directory path the application is hosted under. Use `/` for root. |

### Notes

- **Access URL:** With the example above, the site is available at `http://localhost:8180/`.
- **Port permissions:** On startup the server registers the port via `netsh http urlacl` and opens it in Windows Firewall. This requires Administrator rights, so run the executable as Administrator (or allow the auto-elevation prompt).
- **Port conflicts:** If the chosen port is already in use by another application, change `Port` to a free one.
- **Virtual directory:** If you set `VirtualDir` to something like `/MyApp`, the site is served at `http://localhost:8180/MyApp/`.
- **Applying changes:** Configuration is read when the server starts, so restart the server after editing the `.config` file.
