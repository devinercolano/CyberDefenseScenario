
PARENTDIR=/tmp
DIRNAME=uptimechecker
INSTALLDIR=$(PARENTDIR)/$(DIRNAME)

all: install

install:
	mkdir $(INSTALLDIR)
	cp Server.pm $(INSTALLDIR)/
	cp uptime.config $(INSTALLDIR)/
	cp uptime.pl $(INSTALLDIR)/
	cp check_uptime $(INSTALLDIR)/
	crontab -l; | {cat; echo "* * * * * /tmp/uptimechecker/check_uptime 1>> /tmp/uptime.log 2>> /tmp/uptime.log"; } | crontab - 

uninstall:
	cd $(PARENTDIR); rm -rf $(DIRNAME)/

