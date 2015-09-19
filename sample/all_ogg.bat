@ECHO OFF
REM $Id$
REM カレントディレクトリ内の全ての台詞(*.ogg)を片っ端から再生します
SET PLINK="X:\Program Files\PuTTY\plink.exe"
SET CM3D2_HOST=127.0.0.1
SET CM3D2_PORT=9000

FOR /F "delims=" %%I in ('dir /B *.ogg') do (
	echo VOICE=%%I を実行中
	echo VOICE=%%I | %PLINK% -raw -P %CM3D2_PORT% %CM3D2_HOST%
	pause > NUL
)
