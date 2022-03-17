FILES_TO_UPDATE := $(shell grep -rlE --exclude-dir={{cookiecutter.name}}/Library '{{cookiecutter.clientPackageName}}|{{cookiecutter.description}}|{{cookiecutter.localUrl}}' '{{cookiecutter.name}}')

FILES_TO_UNDO := $(shell grep -rlE --exclude-dir={{cookiecutter.name}}/Library 'minimal-client-description|localhost' '{{cookiecutter.name}}')

local-test:
	@echo "Files to be updated for local test: ${FILES_TO_UPDATE}"
	@for file in ${FILES_TO_UPDATE}; do \
		sed -E -i '' 's/{{cookiecutter\.name}}/minimal-client-template/g' $$file; \
		sed -E -i '' 's/{{cookiecutter\.title}}/Minimal Client Template/g' $$file; \
		sed -E -i '' 's/{{cookiecutter\.description}}/minimal-client-description/g' $$file; \
		sed -E -i '' 's/{{cookiecutter\.localUrl}}/localhost/g' $$file; \
		sed -E -i '' 's/{{cookiecutter\.localPort}}/3250/g' $$file; \
	done

undo-local-test:
	@echo "Files to be updated into template mode: ${FILES_TO_UNDO}"
	@for file in ${FILES_TO_UNDO}; do \
		sed -E -i '' 's/minimal-client-template/{{cookiecutter\.name}}/g' $$file; \
        sed -E -i '' 's/Minimal Client Template/{{cookiecutter\.title}}/g' $$file; \
        sed -E -i '' 's/minimal-client-description/{{cookiecutter\.description}}/g' $$file; \
        sed -E -i '' 's/localhost/{{cookiecutter\.localUrl}}/g' $$file; \
        sed -E -i '' 's/3250/{{cookiecutter\.localPort}}/g' $$file; \
	done

protos: protos-clean protos-lint ## Generate protobuf files
	@prototool generate
	@find . 					\
		-type f -name '*.pb.go' 			\
		-not -path "./vendor/*"			\
		-exec sed -i "" 's/,omitempty//' {} \;
	@echo "Protobuf files generated."

protos-lint:
	@-prototool lint

protos-clean:
	@find . -type f -name '*.pb.cs' -delete
	@find . -type f -name '*.pb.go' -delete