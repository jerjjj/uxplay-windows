#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"

$cert = New-SelfSignedCertificate -Type CodeSigningCert `
    -Subject "CN=UxPlay" `
    -FriendlyName "UxPlay Dev Cert" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -TextExtension @(
        "2.5.29.19={text}CA=false&pathLength=0",
        "2.5.29.37={text}1.3.6.1.5.5.7.3.3"
    )

Write-Host "Thumbprint: $($cert.Thumbprint)"
$pwd = ConvertTo-SecureString -String "uxplay" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "$PSScriptRoot\..\UxPlay_TemporaryKey.pfx" -Password $pwd
Write-Host "Certificate regenerated with Basic Constraints." -ForegroundColor Green
