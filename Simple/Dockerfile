#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5001

RUN apt-get update
RUN apt-get update && \
    apt-get install  -y \
        apt-transport-https \
        arping \
        arptables \
        bridge-utils \
        ca-certificates \
        conntrack \
        curl \
        dnsutils \
        ethtool \
        gdb \
        iperf \
        iperf3 \
        iproute2 \
        nano \
        tar \
        ipset \
        iptables \
        iputils-ping \
        jq \
        kmod \
        ldap-utils \
        less \
        libpcap-dev \
        net-tools \
        netcat \
        netcat-openbsd \
        openssl \
        procps \
        nfs-kernel-server \
        psmisc \
       # python-dbg \
       # python3-dbg \
        socat \
        software-properties-common \
        strace \
        tcpdump \
        telnet \
        tmux \
       # mongodb \
        tree \
        vim \
        wget && \
    rm -rf /var/lib/apt/lists/*

 
#COPY ./simple.pfx /app
COPY ./davetest.pfx /app
#RUN mkdir /etc/secretVolume
#COPY ./special.how /etc/config
#RUN chmod -R 400 /etc/config/


 

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Simple/Simple.csproj", "Simple/"]
RUN dotnet restore "Simple/Simple.csproj"
COPY . .
WORKDIR "/src/Simple"
RUN dotnet build "Simple.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Simple.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Simple.dll"]