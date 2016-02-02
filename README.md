# DialerNetAPIDemo
A simple Website that shows how to use Interactive Intelligence Inc 's Interaction Dialer .Net API

# Installation

## IIS

```powershell
PS> Install-WindowsFeature Web-Server -IncludeAllSubfeature
PS> Install-WindowsFeature Web-Mgmt-Service
PS> Get-NetFirewallRule | Where DisplayGroup -eq 'Web Management Service (HTTP) | Enable-NetFirewallRule
```

## Web Platform Installer

If you use Chocolatey:
```powershell
PS> choco install -y webpicmd
```

If not, 
```powershell
PS> Start-BitsTransfer http://download.microsoft.com/download/C/F/F/CFF3A0B8-99D4-41A2-AE1A-496C08BEB904/WebPlatformInstaller_amd64_en-US.msi $HOME\Downloads
PS> msiexec  /i $HOME\Downloads\WebPlatformInstaller_amd64_en-US.msi /qn /norestart /l*v C:\Windows\Logs\install-wpi.log
```

## Web Deploy
```powershell
PS> webpicmd /Install /Products:"WDeploy"
```

## .Net 3.5:
```powershell
PS> Install-WindowsFeature Net-Framework-Core
```

## Icelib
Install Interactive Intelligence IceLib (to get ININ Trace Services):
```powershell
PS> msiexec /I $HOME\Downloads\IceLibSDK_64bit_2016_R1.msi /qn /l*v C:\Windows\Logs\install-icelib.log
```

Open the Visual Studion Solution and publish via Web Deploy to your web server.
