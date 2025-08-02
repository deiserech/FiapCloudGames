


$originFolder =  Get-Location

cd ..


cd ".\tests\FiapCloudGames.Tests"
$reporttFolder = ".\..\coverage-report"

dotnet clean
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

cd ".\TestResults"


$firstFolder = Get-ChildItem | Where-Object { $_.PSIsContainer } | Select-Object -First 1
reportgenerator -reports:"$($firstFolder.FullName)\coverage.cobertura.xml" -targetdir:"$reporttFolder" -classfilters:"-*Migration*;-*Migrations*"

Invoke-Item "$reportFolder\index.html"
cd $originFolder
