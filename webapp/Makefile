up:
	sudo lsof -t -i tcp:3200 | xargs kill -9
	nx run vex:serve --skip-nx-cache

api:
	npm run generate:api

build:
	npm run build --configuration=production
	