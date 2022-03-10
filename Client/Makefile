FOLDER_PATH := $(dir $(realpath $(firstword $(MAKEFILE_LIST))))
PROJECT_NAME := minimal-client-template

UNITY_VERSION=$$(sed -ne 's/^m_EditorVersion: \(.*$$\)/\1/gp' ${FOLDER_PATH}ProjectSettings/ProjectVersion.txt)
UNITY_DIR ?= "/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app"
ENABLE_LOG = false
CI_COVERAGE := -enableCodeCoverage -coverageResultsPath . -coverageOptions enableCyclomaticComplexity;generateHtmlReport;generateBadgeReport;assemblyFilters:+Wildlife.MetagameBase

SHELL := /bin/bash

.PHONY: 

help: ## Display a help screen for declared commands
	@grep -h -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'
