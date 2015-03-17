param($installPath, $toolsPath, $package, $project) 

foreach ($config in $project.ConfigurationManager)
{
$constants = $config.Properties.Item("DefineConstants").Value + ";REMICONTROL"
$config.Properties.Item("DefineConstants").Value = $constants
}