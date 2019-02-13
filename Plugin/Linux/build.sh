gcc -Wall -Wno-unknown-pragmas \
    -O2 -fPIC -Wl,--gc-sections \
    -I../Include \
    -I../../../NDI\ SDK\ for\ Linux/include \
    ../Source/Callback.cpp \
    ../Source/PluginEntry.cpp \
    -lstdc++ \
    -lndi \
    -shared -o libKlakNDI.so
