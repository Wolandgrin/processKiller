This is a small project building application that monitors launched Windows process and kills it when timeout is reached.
# Verified on Windows 10 using Rider/Visual Studio
# Monitor.exe file is built in the bin/Debugfloder
# For proper app monitoring launch: "Monitor.exe <process name> <process timeout> <check interval>" 
## This command line utility expects three arguments: a process name, its maximum lifetime (in minutes) and a monitoring frequency (in minutes, as well). 
## When you run the program, it starts monitoring processes with the frequency specified. 
## If a process of interest lives longer than the allowed duration, the utility kills the process and adds the corresponding record to the console log.
## Example: monitor.exe notepad 5 1 – checks launched notepad.exe every minute and kills notepad process if it lives longer than 5 minutes

# Build the project
## No specific config or changes should be required, just build the app and use terminal to launch Monitor.exe from bin/Debug folder

# Notes
## Implemented with addition of 1 minute (+ 60000 ms) due to specified "every *other* minute" rule in the task (which means not just EVERY minute like 1, 2, 3, etc but over that minute/once in 2 minutes, e.g. 1, 3, 5)