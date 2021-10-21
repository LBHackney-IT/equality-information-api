.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build equality-information-api

.PHONY: serve
serve:
	docker-compose build equality-information-api && docker-compose up equality-information-api

.PHONY: shell
shell:
	docker-compose run equality-information-api bash

.PHONY: test
test:
	docker-compose build equality-information-api-test && docker-compose up equality-information-api-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: restart-db
restart-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database
	docker-compose up -d test-database
