all:
	@echo "all"

clean:
	@echo "clean"

test:
	@echo "test"

build: 
	@echo "build"

# the nc used is the openbsd variant
con:
	@nc localhost 6379

.PHONY: all clean test build con
