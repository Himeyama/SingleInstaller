if (Test-Path -Path "..\HDS"){
    cd ..\HDS
    ./dev publish
    cd ..\SingleInstaller

    if (Test-Path -Path "resources\HDS") {
        Remove-Item -Recurse -Force "resources\HDS"
    }
    cp -r ..\HDS\HDS\publish resources\HDS
}

Compress-Archive -Path "resources\*" -DestinationPath "resources\package.zip" -Force

if (Test-Path -Path "publish\tmp_installer") {
    Remove-Item -Force "publish\tmp_installer" -Recurse
}

dotnet publish