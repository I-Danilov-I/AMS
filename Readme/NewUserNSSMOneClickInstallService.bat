@echo off
REM Name des Dienstes
set SERVICE_NAME=AMS

REM Pfad zu NSSM
set NSSM_PATH="AMS\Readme\nssm-2.24\win64\nssm.exe"

REM Benutzername für den Dienst
echo Bitte geben Sie den Benutzernamen ein:
set /p SERVICE_USER=

REM Pfad zur ausführbaren Datei des Dienstes
echo Bitte geben Sie den Pfad zur ausführbaren Datei des Dienstes ein:
set /p SERVICE_EXE_PATH=

REM Passwort für den Dienst
echo Bitte geben Sie das Passwort ein:
set /p SERVICE_PASSWORD=

REM Lösche den Dienst, falls er bereits existiert
%NSSM_PATH% status %SERVICE_NAME% > nul 2>&1
if %errorlevel% == 0 (
    %NSSM_PATH% stop %SERVICE_NAME%
    %NSSM_PATH% remove %SERVICE_NAME% confirm
    echo Dienst %SERVICE_NAME% wurde geloescht.
)

REM Installiere den Dienst
%NSSM_PATH% install %SERVICE_NAME% %SERVICE_EXE_PATH%

REM Setze den Benutzer und das Passwort für den Dienst
%NSSM_PATH% set %SERVICE_NAME% ObjectName %SERVICE_USER% %SERVICE_PASSWORD%

REM Startet den Dienst
%NSSM_PATH% start %SERVICE_NAME%

echo Dienst %SERVICE_NAME% wurde erfolgreich installiert und gestartet.
pause
