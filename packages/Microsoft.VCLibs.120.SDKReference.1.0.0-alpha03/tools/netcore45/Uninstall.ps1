param($installPath, $toolsPath, $package, $project)
# $installPath is the path to the folder where the package is installed
# $toolsPath is the path to the tools directory in the folder where the package is installed
# $package is a reference to the package object. 
# $project is a reference to the EnvDTE project object and represents the project the package is installed into. 

# Unloading the project resets $project, which breaks Nuget's uninstallation afterward. So disable it for now.
$canUnloadProject = $false

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Copy some strings (unloading the project empties $project)
$projectName = $dte.Solution.Properties.Item("Name").Value + "\" + $project.Name
$projectUniqueName = $project.UniqueName

$window = $dte.Windows.Item([EnvDTE.Constants]::vsWindowKindSolutionExplorer)

$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($project.FullName) | Select-Object -First 1

if ($msbuild.GetProperty("TargetFrameworkIdentifier").Xml.Value -eq ".NETCore")
{	
	if ($canUnloadProject)
	{
		Write-Host "Saving all files before unloading the project"

		$dte.ExecuteCommand("File.SaveAll", "");
		$project.Save();

		Write-Host ("Unloading " + $projectUniqueName)
		
		$window.Activate()
		$window.Object.GetItem($projectName).Select([EnvDTE.vsUISelectionType]::vsUISelectionTypeSelect)
		$dte.ExecuteCommand("Project.UnloadProject", "")
	
		Write-Host "Removing SDKReference Microsoft.VCLibs"

		$item = $msbuild.Xml.ItemGroups.Items | Where-Object { $_.ItemType -eq "SDKReference" -and $_.Include -eq "Microsoft.VCLibs, version=12.0"} | Select-Object -First 1
		$item.Parent.RemoveAllChildren()
	
		$msbuild.Save()
		[Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadProject($msbuild);
		
		Write-Host ("Reloading " + $projectUniqueName)
	
		$window.Activate()
		$window.Object.GetItem($projectName).Select([EnvDTE.vsUISelectionType]::vsUISelectionTypeSelect)
		$dte.ExecuteCommand("Project.ReloadProject", "")
	}
	else
	{
		Write-Host "Unable to unload the project to cleanly uninstall. Please manually remove the reference to 'Microsoft Visual C++ 2013 Runtime Package for Windows'." -BackgroundColor Blue -ForegroundColor White	
	}
}
else
{
	[Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadProject($msbuild);
}

