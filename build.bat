@echo off
set config=Release

set version=1.0.0
if not "%PackageVersion%" == "" (set version=%PackageVersion%)

%PROGRAMFILES(X86)%\MSBuild\14.0\bin\msbuild.exe pst\pst.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

nuget pack pst.nuspec -NoPackageAnalysis -verbosity detailed -Version %version%