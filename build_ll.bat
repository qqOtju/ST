
SET optimization=-O1
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Tools\Llvm\bin\clang.exe" -S %optimization% -emit-llvm Stuff.cpp
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Tools\Llvm\bin\clang.exe" -S %optimization% -emit-llvm main.cpp
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Tools\Llvm\bin\clang.exe" %optimization% main.ll stuff.ll -o main.exe

