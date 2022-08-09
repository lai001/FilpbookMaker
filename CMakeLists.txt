cmake_minimum_required(VERSION 3.1)
project(FilpbookMaker CSharp)
set(TARGET_NAME "FilpbookMaker")

set(CMAKE_CSharp_FLAGS "/langversion:latest")

add_executable(${TARGET_NAME}
    Program.cs
)

set_target_properties(${TARGET_NAME} PROPERTIES
    LINKER_LANGUAGE CSharp
    VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.6.1"
    VS_DOTNET_REFERENCES "
        Microsoft.CSharp;
        System.Drawing;
        System;
        "
)

set_property(TARGET ${TARGET_NAME}
    PROPERTY VS_PACKAGE_REFERENCES "CommandLineParser_2.9.1"
)
