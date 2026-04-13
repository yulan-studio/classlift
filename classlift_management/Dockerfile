# 使用 .NET SDK 构建镜像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件
COPY . .
# Debug: print where we are and list files

RUN echo "Folder Inside container at:" && pwd && echo "--- ls -al ---" && ls -al
RUN test -f Web/Web.csproj || (echo "必须从 nsns_management/ 目录执行 docker build" && exit 1)
RUN dotnet publish Web/Web.csproj -c Release -o /app/out

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Web.dll"]