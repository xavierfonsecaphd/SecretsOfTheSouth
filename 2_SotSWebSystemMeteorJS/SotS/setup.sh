#! /bin/bash

rm ../Build/SotS.tar.gz* || meteor build ../Build --architecture os.linux.x86_64 && split -b 30m  ../Build/SotS.tar.gz ../Build/"SotS.tar.gz.part"