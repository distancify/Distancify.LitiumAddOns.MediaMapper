function SetVersion ($file, $fileVersion, $packageVersion)
{
    $file = Resolve-Path $file

    if (!$fileVersion) { $fileVersion = "0.0.1" }
    if (!$packageVersion) { $packageVersion = "0.0.1-ci" }

    Write-Output "Updating $file"
    Write-Output "Setting AssemblyVersion to $fileVersion"
    Write-Output "Setting AssemblyFileVersion to $fileVersion"
    Write-Output "Setting AssemblyInformationalVersion to $packageVersion"

    $sr = new-object System.IO.StreamReader( $file, [System.Text.Encoding]::GetEncoding("utf-8") )
    $content = $sr.ReadToEnd()
    $sr.Close()

    $content = [Regex]::Replace($content, "AssemblyVersion\("".*?""\)", "AssemblyVersion(""$fileVersion"")");
    $content = [Regex]::Replace($content, "AssemblyFileVersion\("".*?""\)", "AssemblyFileVersion(""$fileVersion"")");
    $content = [Regex]::Replace($content, "AssemblyInformationalVersion\("".*?""\)", "AssemblyInformationalVersion(""$packageVersion"")");

    $sw = new-object System.IO.StreamWriter( $file, $false, [System.Text.Encoding]::GetEncoding("utf-8") )
    $sw.Write( $content )
    $sw.Close()
}

SetVersion "Distancify.LitiumAddOns.Serilog\Properties\AssemblyInfo.cs" $Env:FILEVERSION $Env:PACKAGEVERSION
