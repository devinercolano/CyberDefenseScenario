#!/usr/bin/perl

# uptime.pl by David Weinman <david@weinman.com>
#
# reads in seconds since our lest check as the only
# argument (apprxsec_sincelastcheck), reads in the
# uptime.config file, makes a Server object out
# of every entry and adds them to an array of Server
# objects, iterates through the array, saves the
# uptime and downtime from their respective files
# and then checks if the server is up or down and
# writes the new uptime and downtime to the uptime
# file associated with that server.

require 5.008;
use strict;
use warnings;
use Socket;
use Server;

use constant TIMEOUT => 8;
use constant MAX_UPTIME_FILE_LEN => 40;
use constant DELIM => ',';

sub ltrim($);
sub rtrim($);

# Left trim function to remove leading whitespace
sub ltrim($)
{
	my $string = shift or error("ltrim got no string to trim");
	$string =~ s/^\s+//;
	return $string;
}

# Right trim function to remove ending whitespace
sub rtrim($)
{
	my $string = shift or error("rtrim got no string to trim");
	$string =~ s/\s+//gm;
	return $string;
}

sub error
{
	my $e = shift;
	print(STDERR "[!] $0: $e\n");
	exit 0;
}

sub errormsg
{
	my $e = shift;
	print(STDERR "[!] $0: $e\n");
}

if (@ARGV != 1 || $ARGV[0] !~ /[0-9]+/) {
	error("I need exactly one argument (seconds since the last uptime check)");
}

my $apprxsec_sincelastcheck = int($ARGV[0]);

# flush after every write
$| = 1;

my @servers;

# get the list of servers 
open(FILE, "<uptime.config") or error("could not open uptime.config: " . $! . "\n");

my $i = 0;
while (<FILE>) {
	my @data = split(DELIM, $_);
	unless (@data != 5) {
		$servers[$i++] = Server->new({
		hostname => ltrim($data[0]),
		portnumber => ltrim($data[1]),
		protocol => ltrim($data[2]),
		hello_msg => ltrim($data[3]),
		output_file => rtrim(ltrim($data[4]))
		});
	} else {
		error("invalid server entry on line " . ($i + 1) . ".");
	}
}

foreach my $val (@servers) {
	print $val->to_string();
	my $isup = 1;
	my($sock);
	my($received_data);
	my($protoname, $protoaliases, $proto_num);
	my $uptime_min = 0; my $uptime_sec = 0;
	my $downtime_min = 0; my $downtime_sec = 0;

	# unless the file doesn't exist, open it in read and get the current uptime and downtime
        unless (! -e $val->{output_file}) {
		# open uptime file (in read mode) and check that the size is within bounds
		open(UPTIMEFH, '<', $val->{output_file}) or error("file open error ($!)");
		my $uptime_fsize = -s $val->{output_file};
		error("invalid uptime file size ($val->{output_file} is " . $uptime_fsize . " bytes)") unless (0 < $uptime_fsize && $uptime_fsize < MAX_UPTIME_FILE_LEN);

		# read in the file
		my $current_uptime;
		$current_uptime = $current_uptime . $_ while <UPTIMEFH>;

		# find the uptime min & sec
		if ($current_uptime =~ /up:\(([0-9][0-9][0-9]?)min, ([0-9][0-9])sec\)/) {
			$uptime_min = $1;
			$uptime_sec = $2;
		} else {
			errormsg("could not find uptime in file for server (" . rtrim($val->to_string()) . "), continuing with any other servers.");
			next;
		}

		# find the downtime min & sec
		if ($current_uptime =~ /down:\(([0-9][0-9][0-9]?)min, ([0-9][0-9])sec\)/) {
			$downtime_min = $1;
			$downtime_sec = $2;
		} else {
			errormsg("could not find downtime in file for server (" . rtrim($val->to_string()) . "), continuing with any other servers.");
			next;
		}

		close(UPTIMEFH);
		# open the uptime file (in write mode)
		open(UPTIMEFH, '>', $val->{output_file}) or error("file open error($!)");
	} elsif (! -e $val->{output_file}) {
		# open uptime file (in write mode)
		open(UPTIMEFH, '>', $val->{output_file}) or error("file open error ($!)");
	}

	if ($val->{protocol} =~ /^udp/i) {
		($protoname, $protoaliases, $proto_num) = getprotobyname('udp');
		socket($sock, AF_INET, SOCK_DGRAM, $proto_num) or $isup = 0;
		my $paddr = sockaddr_in($val->{portnumber}, inet_aton($val->{hostname}));
		connect($sock, $paddr) or $isup = 0;
	} elsif ($val->{protocol} =~ /^tcp/i) {
		($protoname, $protoaliases, $proto_num) = getprotobyname('tcp');
		socket($sock, AF_INET, SOCK_STREAM, $proto_num) or $isup = 0;
		my $paddr = sockaddr_in($val->{portnumber}, inet_aton($val->{hostname}));
		connect($sock, $paddr) or $isup = 0;
	} else {
		error("unknown protocol: $val->{protocol}.\n");
	}
	if ($isup == 0) {
		errormsg("error in socket creation : $!\n\n");
		$uptime_min = ((int($uptime_min) > 9 || length($uptime_min) >= 2) ? "" : "0") . $uptime_min . "min";
		$uptime_sec = ((int($uptime_sec) > 9 || length($uptime_sec) >= 2) ? "" : "0") . $uptime_sec . "sec";
		$downtime_min = $downtime_min + int(($downtime_sec + $apprxsec_sincelastcheck) / 60) unless (($downtime_sec + $apprxsec_sincelastcheck) < 60);
		$downtime_min = ((int($downtime_min) > 9 || length($downtime_min) >= 2) ? "" : "0") . $downtime_min . "min";
		$downtime_sec = (($downtime_sec + $apprxsec_sincelastcheck) % 60);
		$downtime_sec = ((int($downtime_sec) > 9 || length($downtime_sec) >= 2) ? "" : "0") . $downtime_sec . "sec";
	} else {
		# print connection success unless there is no connection process i.e. if we are using udp
		print("[*] Connection success.\n") unless $proto_num eq 17;
		send($sock, "$val->{hello_msg}\r\n\r\n", 0);
		print("[*] sent $val->{hello_msg}\n");
		$received_data = "";
		# if we are dealing with udp, we need a timout in case the server is dead ( or doesn't respond )
		eval {
			# make a subroutine for letting the user know that the alarm timed out
			# and register this subroutine as the handler for the alarm signal
			local $SIG{ALRM} = sub { errormsg("udp server alarm time out"); };
			alarm TIMEOUT; # send an alarm signal after TIMEOUT seconds
			recv($sock, $received_data, 10000, 0);
			alarm 0;
			1;
		} if ($proto_num eq 17);
		# if we are dealing with tcp, we have a connection to rely on, so no timeouts!
		if ($proto_num eq 6) {
			recv($sock, $received_data, 10000, 0);
		}
		if (length($received_data) <= 0) {
			errormsg("received nothing!\n");
			$uptime_min = ((int($uptime_min) > 9 || length($uptime_min) >= 2) ? "" : "0") . $uptime_min . "min";
			$uptime_sec = ((int($uptime_sec) > 9 || length($uptime_sec) >= 2) ? "" : "0") . $uptime_sec . "sec";
			$downtime_min = $downtime_min + int(($downtime_sec + $apprxsec_sincelastcheck) / 60) unless (($downtime_sec + $apprxsec_sincelastcheck) < 60);
			$downtime_min = ((int($downtime_min) > 9 || length($downtime_min) >= 2) ? "" : "0") . $downtime_min . "min";
			$downtime_sec = (($downtime_sec + $apprxsec_sincelastcheck) % 60);
			$downtime_sec = ((int($downtime_sec) > 9 || length($downtime_sec) >= 2) ? "" : "0") . $downtime_sec . "sec";
		} else {
			print("[*] received " . $received_data . "\n\n");
			$uptime_min = $uptime_min + int(($uptime_sec + $apprxsec_sincelastcheck) / 61) unless (($uptime_sec + $apprxsec_sincelastcheck) < 60);
			$uptime_min = ((int($uptime_min) > 9 || length($uptime_min) >= 2) ? "" : "0") . $uptime_min . "min";
			$uptime_sec = (($uptime_sec + $apprxsec_sincelastcheck) % 60);
			$uptime_sec = ((int($uptime_sec) > 9 || length($uptime_sec) >= 2) ? "" : "0") . $uptime_sec . "sec";
			$downtime_min = ((int($downtime_min) > 9 || length($downtime_min) >= 2) ? "" : "0") . $downtime_min . "min";
			$downtime_sec = ((int($downtime_sec) > 9 || length($downtime_sec) >= 2) ? "" : "0") . $downtime_sec . "sec";
		}
		close($sock);
	}
	print(UPTIMEFH "up:($uptime_min, $uptime_sec) down:($downtime_min, $downtime_sec)");
	close(UPTIMEFH);
}
print "[*] done!\n";

