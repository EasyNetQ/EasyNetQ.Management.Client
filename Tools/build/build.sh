#!/bin/bash

function findPackageAssembly {
    local resultvar=$1
    local dir=$2
    local packageName=$3
    local packageVersion=$4
    local assemblyName=$5
    local packageAssembly=$(find "$dir/$packageName/$packageVersion" -name "$assemblyName" 2>/dev/null)
    eval $resultvar="$packageAssembly"
}

function installPackage {
    local dir=$1
    local packageName=$2
    local packageVersion=$3
    echo "Installing '$packageName' version '$packageVersion' in '$dir'..."
    mkdir -p $dir
    local contents='<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>netstandard1.5</TargetFramework></PropertyGroup></Project>'
    local proj="$dir/prj.csproj"
    echo $contents > $proj
    dotnet add "$proj" package $packageName --version $packageVersion --package-directory $dir
}

function getPackageAssemblyPath {
    local resultvar=$1
    local scriptArgsName=$2[@]
    local packagesDir=$3
    local packageName=$4
    local packageVersionArgSwitch=$5
    local assemblyName=$6
    local scriptArgs=("${!scriptArgsName}")
    for item in ${scriptArgs[@]}; do
        if [[ $item == $packageVersionArgSwitch* ]]; then
            local packageVersion=${item/$packageVersionArgSwitch/}
        fi
    done
    if [ -z $packageVersion ]; then
        local packageVersionSearchUrl="https://api-v2v3search-0.nuget.org/autocomplete?id=$packageName"
        local packageVersion=$(curl --silent $packageVersionSearchUrl | jq .data[-1] | sed -e 's/^"//' -e 's/"$//')
    fi
    findPackageAssembly assemblyPath $buildScriptPackagesDir $packageName $packageVersion $assemblyName
    if [ ! -f "$assemblyPath" ]; then
        installPackage $buildScriptPackagesDir $packageName $packageVersion
        findPackageAssembly assemblyPath $buildScriptPackagesDir $packageName $packageVersion $assemblyName
        if [ ! -f "$assemblyPath" ]; then
            echo "Cannot find '$assemblyName' in directory '$packagesDir' for package '$packageName' of version '$packageVersion'"
            exit 1
        fi
    fi
    eval $resultvar="$assemblyPath"
}

scriptDir=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
rootDir=$scriptDir; while [ $(ls -Ald "$rootDir/.git" 2>/dev/null | wc -l) -eq 0 ]; do rootDir="$rootDir/.."; done
buildScriptPackagesDir="$rootDir/Tools/build/pkgs"
allArgs=$@
getPackageAssemblyPath cakeAssemblyPath allArgs $buildScriptPackagesDir 'cake.coreclr' '--cakePackageVersion=' 'Cake.dll'
cakeScript="$scriptDir/build.cake"
gitRemote=$(git remote get-url origin | sed -e 's/.git$//')
srcDir="$rootDir/Source"
artifactsDir="$rootDir/Tools/build/artifacts"
commitHash=$(git rev-parse --short HEAD)

dotnet "$cakeAssemblyPath" "$cakeScript" "--gitRemote=$gitRemote" "--srcDir=$srcDir" "--artifactsDir=$artifactsDir" "--commitHash=$commitHash" "$@"