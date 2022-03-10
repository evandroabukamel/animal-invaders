#!/bin/bash
unity_path=$1 # Receives a Unity.app path
project_path=$2 # Unity project path
test_categories=$3 # categories filter in the format -testCategory "cat1;cat2;..."
coverage=$4 # coverage configuration in the format -enableCodeCoverage -coverageResultsPath <path> -coverageOptions <options>
enable_log=$5 # true or false
additional_args=$6 # additional arguments for Unity command line
# TODO: Add --help

if [ "$enable_log" == true ]; then
    logArg="-logFile -";
fi

$unity_path/Contents/MacOS/Unity -batchMode $logArg -projectPath $project_path -runTests editmode $test_categories -testResults results.xml $coverage $additional_args

TEST_RESULT_CODE=$?

if [ $TEST_RESULT_CODE -eq 0 ]; then
    echo "$(tput setaf 2)Run succeeded, no failures occurred$(tput sgr0)";
    echo "Generating test results output";
    csc $project_path/tools/test-display.cs /out:$project_path/test-display.exe > /dev/null
    mono $project_path/test-display.exe $project_path/results.xml
elif [ $TEST_RESULT_CODE -eq 2 ]; then
    echo "$(tput setaf 1)Run succeeded, some tests failed$(tput sgr0)";
    echo "Generating test results output";
    csc $project_path/tools/test-display.cs /out:$project_path/test-display.exe > /dev/null
    mono $project_path/test-display.exe $project_path/results.xml
elif [ $TEST_RESULT_CODE -eq 3 ]; then
    echo "$(tput setaf 1)Run failure (other failure)$(tput sgr0)";
else
    echo "$(tput setaf 1)Unexpected exit code $TEST_RESULT_CODE$(tput sgr0)";
fi

exit $TEST_RESULT_CODE
