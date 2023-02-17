# .Net build image
# 앱을 컴파일 하는 데 필요한 컴파일러 및 다른 .NET 종속성 포함
# 프로덕션에 이 이미지를 배포하지 않고 빌드에만 사용 함
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
# 빌드 이미지가 따로 있다면 여기서 다시 컴파일 할 필요가 없이 빠르게 실행 가능
# 위에 빌드한 이미지를 현재 이미지에 복사해서 build과정에서 사용했던 컴파일러나 다른 .NET 종속성이 없는
# 순수하게 빌드된 파일만 갖는 이미지를 만든다.
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "NolowaBackendDotNet.dll"]
