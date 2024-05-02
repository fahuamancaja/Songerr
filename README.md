#Songerr
## Description
Songerr is a NET application that provides services for song parsing and searching. It uses varlous services such as "Songerrservice
ParserService`, and `MusicsearchService' to perform its operations.
## Installation
### Prerequisites
- .NET 7.0
- Docker (optional)
### Steps
1. Clone the repository
2. Navigate to the Songerr directory
3. Run `dotnet restore` to restore the packages
4. Run `dotnet build` to build the project
5. Run `dotnet run` to start the application
## Usage
The application can be interacted with through the *SongerrController`. It uses the MediatR library to handle commands and queries.
## Configuration
The application requires configuration for Youtube and Spotify. These can be set in the Youtubesettings" and `Spotifysettings classes
respectively.
## Contribution
If you want to contribute to the project, please make sure to follow the existing coding style and structure. The project uses a
service-based architecture, and new features should be implemented as services when possible.
## License
MIT License
 
