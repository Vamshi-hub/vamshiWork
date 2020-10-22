; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "astorWork for Navisworks"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Astoria Solutions Pte Ltd"
#define MyAppURL "http://greatearth.cloudapp.net/"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{AB670FB6-5F09-4D71-AA53-CFB5EE66EF65}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={code:GetNavisworksManagePath}Plugins\astorWorkNavis2017
DisableDirPage=Yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
InfoBeforeFile=C:\Developer\Projects\astorWork_Web\Dev-1.1.0\astorWorkNavisworks\Assets\Pre-install.txt
InfoAfterFile=C:\Developer\Projects\astorWork_Web\Dev-1.1.0\astorWorkNavisworks\Assets\Post-install.txt
OutputDir=C:\Developer\Projects\astorWork_Web\Dev-1.1.0\astorWorkMVC\BIM_Plugin
OutputBaseFilename=setup
SetupIconFile=C:\Developer\Projects\astorWork_Web\Dev-1.1.0\astorWorkNavisworks\Assets\32.ico
Compression=lzma
SolidCompression=yes
UsePreviousAppDir=False
EnableDirDoesntExistWarning=True
DirExistsWarning=no
AppendDefaultDirName=False
ArchitecturesInstallIn64BitMode=x64 ia64
LicenseFile=C:\Developer\Projects\astorWork_Web\Dev-1.1.0\astorWorkNavisworks\Assets\License.rtf
WizardImageBackColor=$00FF6633
WizardImageFile=C:\Users\brian.yang\Pictures\Icons\BIM\astoria-1024.bmp
ShowLanguageDialog=no
WizardSmallImageFile=C:\Users\brian.yang\Pictures\Icons\BIM\astoria-128.bmp
SignTool=WindowsSignTool

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Autodesk\Navisworks Manage 2015\Plugins\astorWorkNavis2017\*"; DestDir: "{code:GetNavisworksManagePath}Plugins\astorWorkNavis2017"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "C:\Developer\Temp\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"; DestDir: "{tmp}"; DestName: "NetFrameworkInstaller.exe"; Flags: deleteafterinstall

[Icons]
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Run]
Filename: "{tmp}\NetFrameworkInstaller.exe"; Parameters: "/passive /norestart"; Flags: waituntilterminated; Description: "Install .NET Framework if it doesn't exist"; StatusMsg: "Installing .NET Framework 4.5.2. This might take a few minutes..."; Check: Framework45IsNotInstalled

[Code]

function Framework45IsNotInstalled(): Boolean;
var
  bSuccess: Boolean;
  regVersion: Cardinal;
begin
  Result := True;

  bSuccess := RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', regVersion);
  if (True = bSuccess) and (regVersion >= 379893) then begin
    Result := False;
  end;
end;

function GetNavisworksManagePath(Param: String): String;
var
  bSuccess: Boolean;
  sPath: String;
  regVersion: Cardinal;
begin
  Result := 'C:\';

  bSuccess := RegQueryStringValue(HKLM, 'SOFTWARE\Autodesk\Navisworks API Runtime\12\Navisworks Manage', 'Path', sPath);  
  if (True = bSuccess) then begin
    Result := sPath;
  end;
end;