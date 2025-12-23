# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/McWeaponsPlus/*" -Force -Recurse
dotnet publish "./McWeaponsPlus.csproj" -c Release -o "$env:RELOADEDIIMODS/McWeaponsPlus" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location