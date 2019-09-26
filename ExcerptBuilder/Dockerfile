FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /ExcerptBuilder

# Copiar csproj e restaurar dependencias
COPY *.fsproj ./
RUN dotnet restore
COPY 1e004bc400c06f705f07.pdf ./


# Build da aplicacao
COPY . ./
RUN dotnet publish -c Release -o out --self-contained -r linux-x64



# Build da imagem
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /ExcerptBuilder
COPY --from=build-env /ExcerptBuilder/out .
ENTRYPOINT ["dotnet", "ExcerptBuilder.dll"]