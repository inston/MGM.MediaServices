# MGM.MediaServices

Media processing service for MGM. Runs on Linux and uses external tools like FFmpeg and SoX to standardize video and audio formats for digital platforms.

## Features
- Synchronous and Asynchronous job execution
- Progress reporting
- Designed for extensibility (multi-step and plugin jobs in future)
- Network file support (UNC paths)

## Quick Start
1. Clone the repository
2. `dotnet restore`
3. `dotnet run --project src/MGM.MediaServices.Api`

## Architecture
- Minimal API for job submission
- Background workers for processing
- CPU-aware concurrency control

## Future
- Multi-step jobs
- Runtime plugin support
