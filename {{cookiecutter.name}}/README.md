# {{cookiecutter.title}}

This is a started client Unity project with a minimal subset of modules already setup.

- Authentication
- Simple Configuration
- SQL Persistence
- Metagame Base
- Pitaya v2

## Getting Started

### Requirements

* Unity 2019.4.28f1 or later
* Make

### Running the client

In order to run it in your machine, you'll need to first start up the metagame server and connector, including their dependencies. 
Therefore, after cloning the respective metagame server and connector projects, make sure you execute the steps for starting the servers so the client can connect to it.

You will need to execute commands similar to the ones below.

Running the metagame server:
```sh
cd server
make setup
make kill-deps
make deps
make run
```

Running the connector:
```sh
cd ..
cd connector
make setup
make kill-deps
make deps
make run
```

Please check out the metagame server and connector documentations to verify these commands and start the required servers.

Notice a minimal server template for generating a minimal metagame server project is available [here](https://git.topfreegames.com/ring/template/minimal-metagame-template).

Notice a minimal connector template for generating a minimal connector project is available [here](https://git.topfreegames.com/ring/template/minimal-connector-template).
