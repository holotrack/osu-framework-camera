$year = (Get-Date).Year
$monthDay = [int]([String](Get-Date).Month + [String](Get-Date).Day)
$revision = 0

try {
    $request = "https://api.nuget.org/v3/registration5-gz-semver2/osu.framework.camera/index.json"
    $entries = Invoke-WebRequest $request | ConvertFrom-Json | Select-Object -expand items
    $current = $entries.upper.split(".")

    if ( ([int]($current[0]) -eq $year) -and ([int]($current[1]) -eq $monthDay) ) {
        $revision = [int]($current[2]) + 1
    }

    if ($monthDay.length -eq 2) {
       $monthDay = "0" + $monthDay
    }
}
catch {
    Write-Host "An error has occured. The package may not exist yet in NuGet."
}
finally {
    $version = [string]::Format("{0}.{1}.{2}", [string]$year, [string]$monthDay, [string]$revision)
    & dotnet pack ./osu.Framework.Camera/osu.Framework.Camera.csproj -c Release -o output /p:Version=$version
}