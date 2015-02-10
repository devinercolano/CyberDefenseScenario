UPTIME CHECKER
==============

The Uptime Checking program checks whether or not the list of servers (in uptime.config) are up or down. It then uptimes the filename (in uptime.config) associated with each server by adding the difference in time since you last ran uptime to the file. This is the code needed to install and run the uptime checker.

To install, first download the zip file and cd into it with ```mkdir uptime; cd uptime; wget -O uptime_checker.zip http://bit.ly/16PVIDt; unzip uptime_checker.zip; rm uptime_checker.zip```, then run ```make```.
This installs the uptime checker. Then you may run ```cd ..; rm -rf uptime/``` to delete the downloaded files and return you to the directory where you started.
