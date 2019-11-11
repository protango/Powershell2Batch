@echo off
set scriptName=::ScriptName
:setScriptName
	if exist .\%scriptName% (
		set scriptName=%~n0%RANDOM%.ps1
		goto setScriptName
	)

::Payload

Powershell.exe -executionpolicy Bypass -File  %scriptName%

del %scriptName%