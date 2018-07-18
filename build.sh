#!/bin/bash
if test "$OS" = "Windows_NT"
then
  # use .Net

  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi
  
  [ ! -e build.fsx ] && .paket/paket.exe update
  [ ! -e build.fsx ] && packages/FAKE/tools/FAKE.exe --fsiargs -d:RELEASE init.fsx
  packages/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx 
else
  # use mono
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  mono .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
  	exit $exit_code
  fi

  [ ! -e build.fsx ] && mono .paket/paket.exe update
  [ ! -e build.fsx ] && mono packages/FAKE/tools/FAKE.exe init.fsx
  mono packages/FAKE/tools/FAKE.exe $@ --fsiargs -d:RELEASE build.fsx 
fi
