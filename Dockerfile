# base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# gather licenses
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS licenses
WORKDIR /src
RUN dotnet tool install --global dotnet-project-licenses
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY */*.csproj .
RUN dotnet-project-licenses --include-transitive --packages-filter "/(System|xunit|NuGet|Microsoft|coverlet|FluentAssertions).*/" -u -i . -o

# build everything
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY DotOPDS/*.csproj DotOPDS/
COPY DotOPDS.Server/*.csproj DotOPDS.Server/
COPY DotOPDS.Shared/*.csproj DotOPDS.Shared/
COPY DotOPDS.Contract/*.csproj DotOPDS.Contract/
COPY DotOPDS.FileFormat.Fb2/*.csproj DotOPDS.FileFormat.Fb2/
COPY DotOPDS.BookProvider.Inpx/*.csproj DotOPDS.BookProvider.Inpx/
RUN dotnet restore DotOPDS/DotOPDS.csproj
RUN dotnet restore DotOPDS.Server/DotOPDS.Server.csproj

# copy everything else and build app
COPY . .
RUN dotnet build DotOPDS/DotOPDS.csproj -c Release --no-restore
RUN dotnet build DotOPDS.Server/DotOPDS.Server.csproj -c Release --no-restore

# publish
FROM build AS publish
RUN dotnet publish DotOPDS/DotOPDS.csproj -c Release --no-build -o /app/publish
RUN dotnet publish DotOPDS.Server/DotOPDS.Server.csproj -c Release --no-build -o /app/publish

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=licenses /src/licenses.txt .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotOPDS.Server.dll"]
