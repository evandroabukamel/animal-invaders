# {{cookiecutter.title}}

This is a started client Unity project with a minimal subset of modules already setup.

- Authentication
- Simple Configuration
- Configuration
- SQL Persistence
- Metagame Base
- Pitaya v2

## Getting Started

### Requirements

* Unity 2019.4.28f1 or later
* Make

### Running the client

In order to run it in your machine, you'll need to first start up the metagame server and connector, including their dependencies. 
Therefore, after cloning the respective backend project ((server and connector), make sure you execute the steps for starting the servers so the client can connect to it.

You will need to execute commands similar to the ones below.

Running the metagame and connector servers:
```sh
cd server
python3 tasks.py setup
python3 tasks.py kill-deps
python3 tasks.py deps
python3 tasks.py run-metagame
python3 tasks.py run-connector
```

Please check out the metagame server and connector documentations to verify these commands and start the required servers.

Notice a minimal backend template for generating a minimal metagame and connector servers project is available [here](https://git.topfreegames.com/ring/template/minimal-metagame-template).