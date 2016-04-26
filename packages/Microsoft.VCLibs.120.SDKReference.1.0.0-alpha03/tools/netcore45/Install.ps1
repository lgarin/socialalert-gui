param($installPath, $toolsPath, $package, $project)
# $installPath is the path to the folder where the package is installed
# $toolsPath is the path to the tools directory in the folder where the package is installed
# $package is a reference to the package object. 
# $project is a reference to the EnvDTE project object and represents the project the package is installed into. 

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Copy some strings (unloading the project empties $project)
$projectName = $dte.Solution.Properties.Item("Name").Value + "\" + $project.Name
$projectUniqueName = $project.UniqueName
$projectFullName = $project.FullName

$window = $dte.Windows.Item([EnvDTE.Constants]::vsWindowKindSolutionExplorer)

$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($project.FullName) | Select-Object -First 1

if ($msbuild.GetProperty("TargetFrameworkIdentifier").Xml.Value -eq ".NETCore")
{	
	Write-Host "Saving all files before unloading the project"

	$dte.ExecuteCommand("File.SaveAll", "");
	$project.Save();
	
	Write-Host ("Unloading " + $projectUniqueName)

	$window.Activate()
	$window.Object.GetItem($projectName).Select([EnvDTE.vsUISelectionType]::vsUISelectionTypeSelect)
	$dte.ExecuteCommand("Project.UnloadProject", "")

	Write-Host "Removing Microsoft.VClibs.120.SDKReference.placeholder"

	$item = $msbuild.Xml.ItemGroups.Items | Where-Object { $_.ItemType -eq "None" -and $_.Include -eq "Microsoft.VClibs.120.SDKReference.placeholder" }
	if ($item.Length -gt 0) 
	{ 
		$item.Parent.RemoveChild($item)
	}
	Remove-Item (($projectFullName | Split-Path) + "\Microsoft.VClibs.120.SDKReference.placeholder")
	
	Write-Host "Adding SDKReference Microsoft.VCLibs"

	$group = $msbuild.Xml.AddItemGroup();
	$props = New-Object 'System.Collections.Generic.Dictionary[String, String]'
	$props.Add("Name", "Microsoft Visual C++ 2013 Runtime Package for Windows")
	$group.AddItem("SDKReference", "Microsoft.VCLibs, version=12.0", $props)

	$msbuild.Save()
	[Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadProject($msbuild);
		
	Write-Host ("Reloading " + $projectUniqueName)
	
	$window.Activate()
	$window.Object.GetItem($projectName).Select([EnvDTE.vsUISelectionType]::vsUISelectionTypeSelect)
	$dte.ExecuteCommand("Project.ReloadProject", "")
}
else
{
	[Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadProject($msbuild);
}
