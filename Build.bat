@echo on
set PackageVersion=1.0.0
dotnet restore MistwareUtils.csproj
dotnet build   MistwareUtils.csproj
dotnet pack    MistwareUtils.csproj