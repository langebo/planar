echo '######################'
echo '### BUILDING USERS ###'
echo '######################'
dotnet clean ./Users/Users.csproj
dotnet restore ./Users/Users.csproj
dotnet build ./Users/Users.csproj

echo '#########################'
echo '### BUILDING MEETINGS ###'
echo '#########################'
dotnet clean ./Meetings/Meetings.csproj
dotnet restore ./Meetings/Meetings.csproj
dotnet build ./Meetings/Meetings.csproj

echo '#######################'
echo '### BUILDING INVITE ###'
echo '#######################'
dotnet clean ./Invites/Invites.csproj
dotnet restore ./Invites/Invites.csproj
dotnet build ./Invites/Invites.csproj

echo '############################'
echo '### BUILDING SUGGESTIONS ###'
echo '############################'
dotnet clean ./Suggestions/Suggestions.csproj
dotnet restore ./Suggestions/Suggestions.csproj
dotnet build ./Suggestions/Suggestions.csproj

echo '######################'
echo '### BUILDING VOTES ###'
echo '######################'
dotnet clean ./Votes/Votes.csproj
dotnet restore ./Votes/Votes.csproj
dotnet build ./Votes/Votes.csproj