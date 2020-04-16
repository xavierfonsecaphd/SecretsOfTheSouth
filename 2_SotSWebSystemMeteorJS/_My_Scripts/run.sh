#!/bin/sh
 
# starts pm2 and watches the process
pm2 start process.json -i max --watch