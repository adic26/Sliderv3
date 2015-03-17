param($installPath, $toolsPath, $package, $project) 

foreach ($config in $project.ConfigurationManager)
{
$constants = $config.Properties.Item("DefineConstants").Value
$config.Properties.Item("DefineConstants").Value = $constants.Replace(";REMICONTROL", "")
}