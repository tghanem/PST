@echo off
set config=%1
if "%config%" == "" (set config=Release)

set version=1.0.0
if not "%PackageVersion%" == "" (set version=%PackageVersion%)

set nuget= if "%nuget%" == "" (set nuget=nuget)

%PROGRAMFILES(X86)%\MSBuild\14.0\bin\msbuild.exe pst\pst.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

mkdir Build
mkdir Build\lib
mkdir Build\lib\net452

%nuget% pack "pst.nuspec" -NoPackageAnalysis -verbosity details -o Build -Version %version% -p Configuration="%config%"