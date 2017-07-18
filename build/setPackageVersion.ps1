# Get NuGet Version from AssemblyInformationalVersion and set as VSTS task variable
$path = '**\*.nuspec'
$dllRelativePath = "bin\release\net45\" # path changes depending on project type
$year = [System.DateTime]::Now.Year
$specFiles = Get-ChildItem $path -Recurse

foreach($file in $specFiles)
{
	$fileName = $file.Name

	$variableSuffix = $fileName.Replace(".nuspec","").Replace(".","_")
	$dll = $file.fullname.Replace($file.Name, "")
	$dll = $dll + $dllRelativePath + $file.Name.Replace(".nuspec", ".dll")

	# reads AssemblyInformationalVersion attribute value
	$versionFullString = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dll).ProductVersion
	$version = $versionFullString.Split(" ")[0] 
	
	Write-Host "Found " $version " from " $dll " for " $fileName

	# make available to later step
	Write-Output ("##vso[task.setvariable variable=NuGetVersion;]$version")

	# adding suffix allows versions for multiple nuspec files
	Write-Output ("##vso[task.setvariable variable=NuGetVersion_$variableSuffix;]$version")
}