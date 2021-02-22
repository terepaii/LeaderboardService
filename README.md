# LeaderboardService
Leaderboard Service written in C#

![Status](https://github.com/terepaii/LeaderboardService/workflows/.NET/badge.svg)

## Requirements
- Docker 3.1.0

## Setup
### Docker Network
**If using the ELK stack**, the application and logstash need to be on the same docker bridge network. The docker compose files assume `logging` as the network.

Run: `docker network create logging`

### Application Start
The `Service` directory contains a `docker-compose.yml` which starts up:
1. Leaderboard API (**8080**/http)
2. MongoDB (**27017**/mongo)
3. MongoExpress (**8081**/http)
4. Grafana (**3000**/http)
5. Prometheus (**9090**/http)

Each container is communicating on the same docker bridge so can be referred to in configuration and service through the container name.

### ELK Stack
This project uses the docker ELK stack provided by deviantony: [deviantony/docker-elk](https://github.com/deviantony/docker-elk)

#### ELK Stack Start
To start the ELK stack, use the `docker-compose up` in the `docker-elk` submodule. This starts up the following containers and ports:

**Logstash**
 - 5000/tcp/udp - Logstash TCP/UDP input 
 - 5044/http - Logstash beats input
 - 8085/http - Logstash http input for Serilog
 - 9600/http - Logstash monitoring API

**Elasticsearch**
 - 9200/http - Elasticsearch http API
 - 9300/tcp - Elasticsearch tcp input

**Kibana**
 - 5601/http - Kibana Frontend


##Todo
* ~~Standardise API controllers~~
* ~~Change to async functions on server~~
* ~~MongoDB dockerfile~~
* Authentication (Separate Service)
* ~~ELK stack~~
* ~~Metrics~~
* ~~Indexes~~
* Connection Pooling
* Add startup script
* Write to Specific Leaderboard per request
* ~~Interfaces~~
* ~~DI~~
* Redis backend
* ~~Add Unit Tests~~
* Integration Tests
* Loadtesting
* Authentication to mongo
* Terminating Proxy
