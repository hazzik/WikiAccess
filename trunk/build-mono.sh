#!/bin/sh
mkdir mono-out
cd mono-out
gmcs -reference:System.Web.dll -target:library -out:WikiAccess.dll -doc:WikiAccess.xml -nowarn:1591 -nowarn:1587 ../*.cs ../Wikimedia/*.cs
cd ..