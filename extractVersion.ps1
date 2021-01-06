$version = select-string -Path 'Directory.Build.Props' -Pattern '<Version>(.*)<\/Version>' -AllMatches | % { $_.Matches } | % { $_.Groups[1].Value }
echo $version
$env:Version = $version