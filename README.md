# P2P Chat Program
The P2P Chat Program is a .NET-based application designed for creating a peer-to-peer (P2P) chat connection. Leveraging .NET Core, it features a console application and a core library that collectively facilitate secure and efficient P2P chatting capabilities. Key features include network discovery, encrypted messaging, and a user-friendly command-line interface.

## Components
EncryptionService: Manages the encryption and decryption of chat messages.
NetworkDiscovery: Responsible for discovering peers within the network.
P2PChatCore: The core library encompassing the main logic for the P2P chat functionalities.
Console Application: Provides the user interface for interacting with the chat system.

## How to Set Up
Prerequisites: Ensure you have .NET Core 8.0 or later installed.
Clone the Repository: Download the project to your local machine.
Build the Solution: Use Visual Studio to open P2P-Chat-Program.sln and build the solution.

## Usage
Start the Chat Server: Launch the console app with your IP and port.
```bash
P2PChat-Console.exe <Your IP> <Your Port>
```
Connect to a Peer: Add the IP and port of the peer to connect.
```bash
P2PChat-Console.exe <Your IP> <Your Port> <Peer IP> <Peer Port>
```

Chatting: Enter messages into the console to communicate with the connected peer.

## Development
Core Library (P2PChat-Core.csproj): Contains the essential chat logic.
Console Application (P2PChat-Console.csproj): The command-line interface for the chat system.

## License
The project is under the MIT License. Refer to the LICENSE file for more information.