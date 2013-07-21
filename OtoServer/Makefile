srcdir=.
topdir=$(shell pwd)
XBUILD=xbuild
# set to directory of ServiceStack libraries.
# is below the source tree, in lib here.
SSDir=$(realpath ${topdir}/../../lib)

all:
	@echo Top is $(topdir), srcdir is $(srcdir), ServiceStack is at $(SSDir)
	$(XBUILD) $(srcdir)/OtoServer.csproj /t:Build /p:"ReferencePath=$(SSDir)"
