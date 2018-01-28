function Find-PackageAssembly {
    Param($dir, $packageName, $packageVersion, $assemblyName)
    $packagePath = Join-Path $dir (Join-Path $packageName $packageVersion)
    if (-Not (Test-Path $packagePath)) {
        return $null
    }
    return (Get-ChildItem -Force -Recurse $packagePath -Filter $assemblyName).FullName
}

function Install-Package {
    Param($dir, $packageName, $packageVersion)
    Write-Host "Installing '$packageName' version '$packageVersion' in '$dir'..."
    New-Item -Type Directory -Path $dir -Force | Out-Null
    $contents = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>netstandard1.5</TargetFramework></PropertyGroup></Project>'
    $proj = Join-Path $dir 'prj.csproj'
    Out-File -InputObject $contents -FilePath $proj
    $addPackageOutput = $(dotnet add "$proj" package $packageName --version $packageVersion --package-directory $dir)
    Write-Host $addPackageOutput
}

function Get-PackageAssemblyPath {
    Param($scriptArgs, $packagesDir, $packageName, $packageVersionArgSwitch, $assemblyName)
    $packageVersionArg = $scriptArgs | Where-Object { $_ -ne $null -and $_.StartsWith($packageVersionArgSwitch) }
    if ($packageVersionArg -eq $null) {
        $packageVersion = (Find-Package -Source 'http://www.nuget.org/api/v2' -Name $packageName).Version
    }
    else {
        $packageVersion = $packageVersionArg.Split('=')[1]
    }
    $assemblyPath = Find-PackageAssembly $packagesDir $packageName $packageVersion $assemblyName
    if (($assemblyPath -eq $null) -or -Not (Test-Path $assemblyPath)) {
        Install-Package $packagesDir $packageName $packageVersion
        $assemblyPath = Find-PackageAssembly $packagesDir $packageName $packageVersion $assemblyName
        if (($assemblyPath -eq $null) -or -Not (Test-Path $assemblyPath)) {
            Throw "Cannot find '$assemblyName' in directory '$packagesDir' for package '$packageName' of version '$packageVersion'"
            exit 1
        }
    }
    return $assemblyPath
}

$scriptDir = $PSScriptRoot
$rootDir = $scriptDir; while ($(Get-ChildItem -Force -Filter '.git' $rootDir | Measure-Object).Count -eq 0) { $rootDir = Join-Path $rootDir '..' }
$buildScriptPackagesDir = Join-Path $rootDir (Join-Path 'Tools' (Join-Path 'build' 'pkgs'))
$cakeAssemblyPath = Get-PackageAssemblyPath $args $buildScriptPackagesDir 'cake.coreclr' '--cakePackageVersion=' 'Cake.dll'
$cakeScript = Join-Path $scriptDir 'build.cake'
$gitRemote=$(git remote get-url origin) -replace '.git$', ''
$srcDir = Join-Path $rootDir 'Source'
$artifactsDir = Join-Path $rootDir (Join-Path 'Tools' (Join-Path 'build' 'artifacts'))
$commitHash=$(git rev-parse --short HEAD)

dotnet "$cakeAssemblyPath" "$cakeScript" "--gitRemote=$gitRemote" "--srcDir=$srcDir" "--artifactsDir=$artifactsDir" "--commitHash=$commitHash" $args