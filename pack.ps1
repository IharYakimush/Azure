dotnet build -c Release
dotnet pack .\Community.Azure.Cosmos\Community.Azure.Cosmos.csproj -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -o .\nupkg