@echo off
dotnet build src/Limbo.Umbraco.Signatur --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget