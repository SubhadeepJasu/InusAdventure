gcc -std=c11 -fPIC -c -I./subprojects/godot-headers src/simple_main.c -o build/simple.o
gcc -rdynamic -shared build/simple.o -o build/libsimple.so
