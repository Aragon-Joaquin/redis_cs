all:
	@echo "all"

clean:
	@echo "clean"

test:
	@echo "test"

build: 
	@echo "build"

debug:
	@dotnet build -c Debug && $(HOME)/opt/netcoredbg/netcoredbg --interpreter=cli -- dotnet ./bin/Debug/net10.0/redis_cs.dll

# the nc used is the openbsd variant
con:
	@nc localhost 6379

.PHONY: all clean test build con
