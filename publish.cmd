nuget restore
msbuild Microsoft.Bot.Sample.QnABot.sln -p:DeployOnBuild=true -p:PublishProfile=psChatBot-Web-Deploy.pubxml -p:Password=AE2lEyivwixAFlFpesv24musWdPZ6rkGYtfRAMC1TktoaweyiZc06D76dhsl

