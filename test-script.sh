#/bin/sh!

dotnet restore
dotnet build
dotnet test --collect:"XPlat Code Coverage" -- ./coverage

curl -Os https://uploader.codecov.io/latest/linux/codecov

chmod +x codecov
./codecov
