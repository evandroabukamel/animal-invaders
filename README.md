# Minimal Client Template

This is a minimal cookiecutter client template that will create a metagame client (Unity) with minimal features:

- Authentication
- Simple Configuration
- SQL Persistence
- Metagame Base
- Pitaya v2

## Using the template

You can use the template with the following commands:

```sh
python3 -m pip install cookiecutter
python3 -m cookiecutter git+ssh://git@git.topfreegames.com/ring/template/minimal-client-template.git
```

You can also use Homebrew to install cookiecutter, running the following commands:

```sh
brew install cookiecutter
cookiecutter git@git.topfreegames.com:ring/template/minimal-client-template.git
```

Fill out the prompted parameters and you'll have a working Unity client!
Standard cookiecutter parameter values are sufficient to get it running locally and staging (wildlife), 
however you will need to set a URL and a port for running it in production (prod).

## Local development

If you're developing locally on the template, you'll notice that the IDE won't work
since we're using placeholders like `{{cookiecutter.name}}` inside the codebase. 
Unity project also will fail to open because package.json has placeholders such as `{{cookiecutter.title}}` 
which are invalid character sequences for a package.json file.

However, you can run `make local-test` in order to enable local development. After that, you can freely
edit the code, and when you're ready to commit, just run `make undo-local-test` beforehand.