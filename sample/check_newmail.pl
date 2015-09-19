#!/usr/bin/perl
# $Id$
# 新着メールが届いたら

use strict;
use lib './lib';
use Net::IMAP::Client;
use IO::Socket::SSL qw( SSL_VERIFY_NONE );
use Data::Dumper;
use Encode;
use Encode::IMAPUTF7;
use IO::Socket;

use constant {
    ### メールサーバ接続の設定
    MAIL_SERVER =>      'mail.example.net',
    MAIL_SERVER_PORT =>  993,
    MAIL_USERNAME =>    'account@for.mail.server',
    MAIL_PASSWORD =>	'yourpassword',
    MAIL_FOLDER =>      'INBOX',
    MAIL_CHECK_INT =>   600,# 秒ごとにチェック

    ### CM3D2.RemoteControl.Plugin
    CM3D2_HOST =>       '127.0.0.1',
    CM3D2_PORT =>       9000,
};

###
my $UIDNEXT = undef;
sub check_new_mail {
    ### http://search.cpan.org/~ganglion/Net-IMAP-Client-0.9505/lib/Net/IMAP/Client.pm
    print "Checking mailbox...";
    my $imap = Net::IMAP::Client->new (
        server  => MAIL_SERVER,
        port    => MAIL_SERVER_PORT,
        user    => MAIL_USERNAME,
        pass    => MAIL_PASSWORD,
        ssl     => 1,
        ssl_options => {
             verify_hostname => 0,
             SSL_verify_mode => SSL_VERIFY_NONE,
        },
    ) or die $@;
    $imap->login or die $imap->last_error;
    my $folder = $imap->status( MAIL_FOLDER );
    #print Dumper( $folder );
    $imap->logout;

    my $ret = defined $UIDNEXT && $UIDNEXT != $folder->{UIDNEXT};
    $UIDNEXT = $folder->{UIDNEXT};
    $ret;
}

###
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

###
one_shot( 'MOTION=maid_stand01 FACE=通常 FACE_BLEND=頬０涙０' );
while( 1 ){
    if( check_new_mail()){
        print " new mails arrive.";
        one_shot( 'MOTION=turn01 FACE=にっこり VOICE=n0_00006.ogg' );
    }
    print "\n";
    sleep MAIL_CHECK_INT;
}
