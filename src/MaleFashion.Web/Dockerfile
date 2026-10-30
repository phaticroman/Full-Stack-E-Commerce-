# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src


RUN apt-get update && apt-get install -y nodejs && rm -rf /var/lib/apt/lists/*

# সব কটি csproj ফাইল কপি করা (লেয়ার ক্যাশিং নিশ্চিত করতে)
COPY ["src/MaleFashion.Web/MaleFashion.Web.csproj", "MaleFashion.Web/"]
COPY ["src/MaleFashion.Domain/MaleFashion.Domain.csproj", "MaleFashion.Domain/"]
COPY ["src/MaleFashion.Application/MaleFashion.Application.csproj", "MaleFashion.Application/"]
COPY ["src/MaleFashion.Infrastructure/MaleFashion.Infrastructure.csproj", "MaleFashion.Infrastructure/"]


RUN dotnet restore "MaleFashion.Web/MaleFashion.Web.csproj"


COPY ./src/ .
WORKDIR "/src/MaleFashion.Web"
RUN dotnet build "MaleFashion.Web.csproj" -c Release -o /app/build

# 2. Publish Stage
FROM build AS publish
RUN dotnet publish "MaleFashion.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .


EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80

ENTRYPOINT ["dotnet", "MaleFashion.Web.dll"]