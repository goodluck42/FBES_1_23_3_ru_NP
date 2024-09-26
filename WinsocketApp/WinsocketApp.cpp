#include <iostream>
#include <vector>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <thread>

#pragma comment(lib, "ws2_32.lib")

constexpr int BUFFER_SIZE = 1024;

int main(int argc, char* argv[])
{
    WORD version = MAKEWORD(2, 2);
    WSADATA data;

    if (WSAStartup(version, &data) != 0)
    {
        return WSAGetLastError();
    }

    addrinfo socketAddrInfo;

    // memset
    ZeroMemory(&socketAddrInfo, sizeof(socketAddrInfo));

    socketAddrInfo.ai_family = AF_INET;
    socketAddrInfo.ai_socktype = SOCK_STREAM;
    socketAddrInfo.ai_protocol = IPPROTO_TCP;

    SOCKET serverSocket = socket(socketAddrInfo.ai_family, socketAddrInfo.ai_socktype, socketAddrInfo.ai_protocol);

    if (serverSocket == INVALID_SOCKET)
    {
        WSACleanup();

        return WSAGetLastError();
    }

    addrinfo* serverAddrInfo;

    if (int result = getaddrinfo("127.0.0.1", "13370", &socketAddrInfo, &serverAddrInfo); result != 0)
    {
        freeaddrinfo(serverAddrInfo);
        WSACleanup();

        return result;
    }

    if (int result = bind(serverSocket, serverAddrInfo->ai_addr, (int)serverAddrInfo->ai_addrlen); result != 0)
    {
        freeaddrinfo(serverAddrInfo);
        WSACleanup();

        return result;
    }

    if (int result = listen(serverSocket, SOMAXCONN); result != 0)
    {
        freeaddrinfo(serverAddrInfo);
        WSACleanup();

        return result;
    }

    std::vector<SOCKET> clientSockets;
    std::vector<std::thread> threads;
    
    while (true)
    {
        // sockaddr_in clientAddr;
        // int addrlen;

        SOCKET clientSocket = accept(serverSocket, NULL, NULL);

        if (clientSocket == INVALID_SOCKET)
        {
            for (auto& socket : clientSockets)
            {
                closesocket(socket);
            }
            
            break;
        }
        
        clientSockets.push_back(clientSocket);

        std::thread thread
        {
            [](SOCKET clientSocket)
            {
                std::unique_ptr<char[]> buffer{new char[BUFFER_SIZE]};

                int read{};

                while (true)
                {
                    std::memset(buffer.get(), 0, BUFFER_SIZE);
                    
                    read = recv(clientSocket, buffer.get(), BUFFER_SIZE, 0);

                    if (read <= 0)
                    {
                        break;
                    }
                                        
                    auto time = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());
                    
                    std::cout << '[' <<  time << "]: " << buffer << '\n';

                    
                }

                closesocket(clientSocket);
            },
            clientSocket
        };

        threads.push_back(std::move(thread));
    }

    freeaddrinfo(serverAddrInfo);
    WSACleanup();
    
    return 0;
}
