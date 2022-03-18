## PATTERNS AND REPLACE VALUES

PACKAGE_PATTERN := {{cookiecutter.clientPackageName}}
PACKAGE_REPLACE := com.wildlifestudios.minimal-client

NAME_PATTERN := {{cookiecutter.name}}
NAME_REPLACE := minimal-client-template

TITLE_PATTERN := {{cookiecutter.title}}
TITLE_REPLACE := Minimal Client Template

DESCRIPTION_PATTERN := {{cookiecutter.description}}
DESCRIPTION_REPLACE := minimal-client-description

LOCAL_URL_PATTERN := {{cookiecutter.localUrl}}
LOCAL_URL_REPLACE := localhost

LOCAL_PORT_PATTERN := {{cookiecutter.localPort}}
LOCAL_PORT_REPLACE := 3250

## REPLACE COMMANDS (DO AND UNDO)

PACKAGE_REPLACE_STR := s_$(PACKAGE_PATTERN)_$(PACKAGE_REPLACE)_g
PACKAGE_UNDO_REPLACE_STR := s_$(PACKAGE_REPLACE)_$(PACKAGE_PATTERN)_g

NAME_REPLACE_STR := s_$(NAME_PATTERN)_$(NAME_REPLACE)_g
NAME_UNDO_REPLACE_STR := s_$(NAME_REPLACE)_$(NAME_PATTERN)_g

TITLE_REPLACE_STR := s_$(TITLE_PATTERN)_$(TITLE_REPLACE)_g
TITLE_UNDO_REPLACE_STR := s_$(TITLE_REPLACE)_$(TITLE_PATTERN)_g

DESCRIPTION_REPLACE_STR := s_$(DESCRIPTION_PATTERN)_$(DESCRIPTION_REPLACE)_g
DESCRIPTION_UNDO_REPLACE_STR := s_$(DESCRIPTION_REPLACE)_$(DESCRIPTION_PATTERN)_g

LOCAL_URL_REPLACE_STR := s_$(LOCAL_URL_PATTERN)_$(LOCAL_URL_REPLACE)_g
LOCAL_URL_UNDO_REPLACE_STR := s_$(LOCAL_URL_REPLACE)_$(LOCAL_URL_PATTERN)_g

LOCAL_PORT_REPLACE_STR := s_$(LOCAL_PORT_PATTERN)_$(LOCAL_PORT_REPLACE)_g
LOCAL_PORT_UNDO_REPLACE_STR := s_$(LOCAL_PORT_REPLACE)_$(LOCAL_PORT_PATTERN)_g

## FIND FILES TO BE UPDATED
COOKIECUTTER_TAGS := '{{cookiecutter.clientPackageName}}|{{cookiecutter.name}}|{{cookiecutter.title}}|{{cookiecutter.description}}|{{cookiecutter.localUrl}}|{{cookiecutter.loaclPort}}'
IGNORE_PATHS := -not -path "*/Library*" -not -path "*/Logs*" -not -path "*/obj*"
FILE_FILTER := -name "*.cs" -or -name "*/Packages/manifest" -or -name "Makefile" -or -name "package.json" -or -name "packages-lock.json"
MATCHING_FILES := $(shell find '{{cookiecutter.name}}' ${IGNORE_PATHS} ${FILE_FILTER})

local-test:
	@echo "Files to be updated for local test: ${MATCHING_FILES}"
	@for file in ${MATCHING_FILES}; do \
		sed -i '' "$(PACKAGE_REPLACE_STR)" $$file; \
		sed -i '' "$(NAME_REPLACE_STR)" $$file; \
		sed -i '' "$(TITLE_REPLACE_STR)" $$file; \
		sed -i '' "$(DESCRIPTION_REPLACE_STR)" $$file; \
		sed -i '' "$(LOCAL_URL_REPLACE_STR)" $$file; \
		sed -i '' "$(LOCAL_PORT_REPLACE_STR)" $$file; \
	done

undo-local-test:
	@echo "Files to be updated into template mode: ${MATCHING_FILES}"
	@for file in ${MATCHING_FILES}; do \
		sed -i '' "$(PACKAGE_UNDO_REPLACE_STR)" $$file; \
        sed -i '' "$(NAME_UNDO_REPLACE_STR)" $$file; \
        sed -i '' "$(TITLE_UNDO_REPLACE_STR)" $$file; \
        sed -i '' "$(DESCRIPTION_UNDO_REPLACE_STR)" $$file; \
        sed -i '' "$(LOCAL_URL_UNDO_REPLACE_STR)" $$file; \
        sed -i '' "$(LOCAL_PORT_UNDO_REPLACE_STR)" $$file; \
	done