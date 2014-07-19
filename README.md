SALTR Unity SDK
===============

This is the README file of the SALTR Unity SDK.

To check out the project from GitHub:
<a href="https://github.com/plexonic/saltr-unity-sdk.git">https://github.com/plexonic/saltr-unity-sdk.git</a>

To download the latest ZIP:
<a href="https://github.com/plexonic/saltr-unity-sdk/archive/master.zip">https://github.com/plexonic/saltr-unity-sdk/archive/master.zip</a>



CONTENTS
========
1. INTRODUCTION
2. USAGE
3. DIRECTORY STRUCTURE
4. DOCUMENTATION

----

1. INTRODUCTION
===============

SALTR Unity SDK is a library of classes which will help to develop mobile 
games that are to be integrated with SALTR platform.

SDK performs all necessary and possible action with SALTR REST API to connect, update, set 
and download data related to application's or game's  features or levels.

All data received from SALTR REST API is parsed and represented through set of instances of classes, 
each carrying specific objects and its properties.

Basically SDK, as the REST API, has few simple actions. The most important one is connecting, 
which loads the app data objects containing features, experiments and level headers.

This and other actions will be described in the sections below.


2. USAGE
========

To use the SDK you need to download/checkout SDK repository and then import files to your
project.

The entry point is the SLTUnity class, which you can create and initialize in code. Or you can darg and drop the SALTR prefab on your main scene and access its instance through SaltrWrapper component.

Note: All classes in the package start with "SLT" prefix.

3. DIRECTORY STRUCTURE
======================

The SDK has the following directory structure:

- assets - contains the SALTR prefab and the wrapper script
- src - root of the library;
- src/saltr - main package of library;
- src/saltr/game - package contains game related classes;
- src/saltr/game/cavas2d - classes related to 2D games;
- src/saltr/game/matching - classes related to matching or board based games;
- src/saltr/game/repository - local data repository classes (implementation widely varies through platforms);
- src/saltr/game/status - status classes representing warnings and error statuses withing library code;
- src/saltr/game/utils - helper or utility classes;

4. DOCUMENTATION
================

Detailed documentation for all public classes and methods is coming soon.
