#!/usr/bin/perl

# Server.pm by David Weinman

package Server;

use strict;
use warnings;

# init server with hostname, portnumber, and protocol
sub new {
	my ($class, $args) = @_;
	my $self = bless { hostname => $args->{hostname},
	portnumber => $args->{portnumber},
	protocol => $args->{protocol},
	hello_msg => $args->{hello_msg},
	output_file => $args->{output_file}
	}, $class;
}

sub to_string {
	my $self = shift;
	return "Server: ip: $self->{hostname} port: $self->{portnumber} protocol: $self->{protocol}\n";
}

1;

