$username = 'machinename\vagrant'
$password = ConvertTo-SecureString -AsPlainText -Force -String 'vagrant'
$credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $password

Test-WSMan -ComputerName 127.0.0.1 -Port 2201 -Authentication Negotiate -Credential $credentials
Connect-WSMan -ComputerName 127.0.0.1 -Port 2201 -Authentication Negotiate -Credential $credentials
Disconnect-WSMan -ComputerName 127.0.0.1

$session = New-PSSession -ComputerName 127.0.0.1 -Port 2201 -Credential $credentials -Authentication Negotiate
Invoke-Command -ScriptBlock { hostname } -Session $session
Enter-PSSession -Session $session
Exit-PSSession
Remove-PSSession $session

New-Item "C:\SharedWithHostOs" –type Directory
Copy-Item -Path 'C:\hostFile' -Destination 'C:\VmFolderSharedWithHostOs' -ToSession $session
Copy-Item -Path 'C:\VmFolderSharedWithHostOs\guestFile' -Destination 'C:\aFile' -FromSession $session
