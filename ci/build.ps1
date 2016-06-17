# Generate the installer
$appVersion = "1.0.0"
$setupName = "orthocite-v$appVersion-x86-setup"
& "C:\Program Files (x86)\Inno Setup 5\iscc" /dMySourceDir="$PSScriptRoot" /dMyAppVersion="$appVersion" /dMyAppOutput="$setupName" $PSScriptRoot\script.iss

# Push the installer to artifacts on Appveyor only
if ($env:APPVEYOR -eq "True") {
  Push-AppveyorArtifact "$PSScriptRoot\output\$setupName.exe" -FileName "$setupName.exe" -DeploymentName "OrthoCité v$packageVersion for Windows x86"
}
