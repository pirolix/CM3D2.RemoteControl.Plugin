#!/usr/bin/perl
# $Id$
# 5秒ごとに任意のMOTIONを実行する

use strict;
use warnings;
use IO::Socket;

# CM3D2.RemoteControl.Plugin
use constant {
    CM3D2_HOST =>        '127.0.0.1',
    CM3D2_PORT =>        9000,
};

sub one_shot {
    my $command = shift or return;
    my $cm3d2 = IO::Socket::INET->new(
        PeerAddr => CM3D2_HOST,
        PeerPort => CM3D2_PORT,
        Proto    => 'tcp',
        TimeOut  => 5,
    ) or die $!;
    print $cm3d2 "$command\n";
    $cm3d2->close;
}

my @motion = qw/
    jump_s
    maid_comehome4_gatsu_once_
    maid_ojigi02_once_
    maid_stand02akubi_once_
    maid_stand02hair_once_
    maid_stand02kaiwa1_once_
    maid_stand02kaiwa2_once_
    maid_stand02listenb_once_
    maid_stand02listenloop3_once_
    maid_stand02ojigi_once_
    maid_stand02sian2_once_
    maid_stand02tameiki_once_
    sys_munehide
    turn01
/;

while( 1 ){
    my $command = 'MOTION='. $motion[ int rand scalar @motion ];
    print $command, "\n";
    one_shot( $command );
    sleep 5;
}
