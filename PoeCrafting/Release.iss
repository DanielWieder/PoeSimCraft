; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{67C429BF-6E31-4381-91D1-FE82A20CA109}
AppName=PoeSimCraft
AppVersion=0.2
;AppVerName=PoeSimCraft 0.2
AppPublisher=Daniel Wieder
AppPublisherURL=https://github.com/DanielWieder/PoeSimCraft
AppSupportURL=https://github.com/DanielWieder/PoeSimCraft
AppUpdatesURL=https://github.com/DanielWieder/PoeSimCraft
DefaultDirName={pf}\PoeSimCraft
DisableProgramGroupPage=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\danie\Documents\GitHub\PoeSimCraft\PoeCrafting\PoeCrafting\bin\Release\PoeCrafting.UI.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\danie\Documents\GitHub\PoeSimCraft\PoeCrafting\PoeCrafting\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\PoeSimCraft"; Filename: "{app}\PoeCrafting.UI.exe"
Name: "{commondesktop}\PoeSimCraft"; Filename: "{app}\PoeCrafting.UI.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\PoeCrafting.UI.exe"; Description: "{cm:LaunchProgram,PoeSimCraft}"; Flags: nowait postinstall skipifsilent
