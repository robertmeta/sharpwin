.PHONY: install
install: release
	cp -rf bin/release/net8.0/* ../
	cp -f ../sharpwin.exe ../log-sharpwin.exe

.PHONY: release
release:
	dotnet build SharpWin.sln -c Release  > build.log

.PHONY: debug
debug:
	dotnet build SharpWin.sln  > build.log

.PHONY: install-debug
install-deubg: debug
	cp -rf bin/debug/net8.0/* ../
	cp -f ../sharpwin.exe ../log-sharpwin.exe

