; ====================================================
; UxPlay Client — Inno Setup Installer Script
; ====================================================
; Usage:
;   ISCC.exe /DPublishDir=<path> /DAppVersion=1.0.0 installer.iss
;
; Prerequisites:
;   1. Inno Setup 6  — https://jrsoftware.org/issetup.exe
;   2. Run build.ps1 first to publish the app
; ====================================================

#ifndef PublishDir
  #define PublishDir "bin\x64\Release\net10.0-windows10.0.22621.0\win-x64\publish"
#endif
#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

[Setup]
AppName=UxPlay Client
AppVersion={#AppVersion}
AppPublisher=UxPlay
AppPublisherURL=https://github.com/jerjjj/libuxplay
DefaultDirName={autopf}\UxPlayClient
DefaultGroupName=UxPlay Client
OutputBaseFilename=UxPlayClient-{#AppVersion}-setup
Compression=lzma2/ultra64
SolidCompression=yes
PrivilegesRequired=admin
OutputDir=Output
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=
UninstallDisplayIcon={app}\UxPlayClient.exe
LicenseFile=
DisableProgramGroupPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
; Chinese overrides for key installer UI strings
WelcomeLabel1=欢迎安装 UxPlay Client
WelcomeLabel2=这将在您的计算机上安装 UxPlay Client {#AppVersion}。%n%n建议关闭所有其他应用程序后再继续。
SelectDirLabel3=安装程序将把 UxPlay Client 安装到以下文件夹。
ReadyLabel1=安装程序已准备好将 UxPlay Client 安装到您的计算机。
FinishedLabel=UxPlay Client 已成功安装。
ButtonNext=下一步(&N) >
ButtonInstall=安装(&I)
ButtonFinish=完成(&F)

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; All published files
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; uxplaylib.dll — user must provide this separately if not in publish dir
; Source: "uxplaylib.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: FileExists(ExpandConstant('{src}\uxplaylib.dll'))

[Icons]
Name: "{group}\UxPlay Client"; Filename: "{app}\UxPlayClient.exe"
Name: "{group}\卸载 UxPlay Client"; Filename: "{uninstallexe}"
Name: "{autodesktop}\UxPlay Client"; Filename: "{app}\UxPlayClient.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\UxPlayClient.exe"; Description: "启动 UxPlay Client"; Flags: nowait postinstall skipifsilent

[Code]
// Check if Windows App Runtime is installed
function IsWindowsAppRuntimeInstalled(): Boolean;
var
  ResultCode: Integer;
begin
  Result := RegKeyExists(HKLM, 'SOFTWARE\Microsoft\WindowsAppRuntime');
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    if not IsWindowsAppRuntimeInstalled() then
    begin
      MsgBox('提示：运行此应用可能需要安装 Windows App Runtime。'#13#10
             + '如果应用无法启动，请从以下地址下载安装：'#13#10
             + 'https://aka.ms/windowsappsdk/1.6/latest/windowsappruntime-installer-x64',
             mbInformation, MB_OK);
    end;
  end;
end;
