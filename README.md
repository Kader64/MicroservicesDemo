# MicroservicesDemo

A minimal demo of a microservices setup built with ASP.NET Core minimal APIs. Two small, independently containerised services run side by side, and one of them calls the other over HTTP using its container hostname.

## Architecture

```
            GET /service1                 GET /
Client ───────────────────▶ Service 2 ───────────────────▶ Service 1
                       "Response from Service 1: ..."   "Hello from Service 1!"
```

Service 2 reaches Service 1 at `http://service1:8080/`, so both services must run on the same container network and Service 1 must be reachable under the hostname `service1` on port `8080`.

## Tech stack

- C# / ASP.NET Core minimal APIs
- `IHttpClientFactory` for service-to-service HTTP calls
- Docker (one Dockerfile per service)
- GitHub Actions (CI)

## Getting started

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [.NET SDK](https://dotnet.microsoft.com/download) (only needed to run the services outside Docker)

### Clone

```bash
git clone https://github.com/Kader64/MicroservicesDemo.git
cd MicroservicesDemo
git checkout develop
```

### Run with Docker

Both services have their own Dockerfile. Because Service 2 resolves `service1` by hostname, the containers must share a user-defined Docker network and Service 1 must have the name/alias `service1`.

<!-- TODO: replace with the exact commands or docker-compose instructions used in this repo, e.g. `docker compose up --build` -->

```bash
docker network create microservices-demo

# Service 1 (container name = hostname used by Service 2)
docker build -t service1 ./<path-to-service1>
docker run -d --name service1 --network microservices-demo service1

# Service 2
docker build -t service2 ./<path-to-service2>
docker run -d --name service2 --network microservices-demo -p 8081:8080 service2
```

### Try it

```bash
curl http://localhost:8081/            # Hello from Service 2!
curl http://localhost:8081/service1    # Response from Service 1: Hello from Service 1!
```

> Adjust the published port to match your configuration.

## Known limitations

- **Startup order:** `GET /service1` fails if Service 1 is not up yet. There is no retry or resilience handling at the moment (see the `TODO` in Service 2's `Program.cs`). Possible fixes: retry/circuit breaker policies (e.g. Polly or `Microsoft.Extensions.Http.Resilience`), or health checks with `depends_on: condition: service_healthy`.
- The Service 1 URL is hard-coded; it could be moved to configuration or an environment variable.

## CI

GitHub Actions builds and tests the project on pushes and pull requests.


```
MicroservicesDemo/
├── <service1>/        # Service 1 source + Dockerfile
├── <service2>/        # Service 2 source + Dockerfile
└── .github/workflows/ # CI pipeline
```
