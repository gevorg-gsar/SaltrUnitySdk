mkdir out
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath $1 -exportPackage Assets $2 -quit
osascript -e 'tell application "Terminal" to quit' &
exit