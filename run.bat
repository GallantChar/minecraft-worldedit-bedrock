del *.csv
cd src\ShapeGenerator\bin\Debug
#rem line in x coordinate
shapegenerator.exe -s line   --x1 10 --z1 10  --y1 4  --x2 50  --y2 4 --z2  10 --block wool
#rem line in z coordinate
shapegenerator.exe -s line   --x1 10 --z1 10  --y1 4  --x2 10  --y2 4 --z2  50
#rem line in y coordinate
shapegenerator.exe -s line   --x1 10 --z1 10  --y1 4  --x2 10  --y2 50 --z2  10 --block wool

shapegenerator.exe -s line   --x1 100 --z1 100  --y1 10  --x2 150  --y2 40 --z2  50
shapegenerator.exe -s line   --x1 100 --z1 100  --y1 10  --x2 150  --y2 40 --z2  150
shapegenerator.exe -s line   --x1 100 --z1 100  --y1 10  --x2 50   --y2 40 --z2  50

shapegenerator.exe -s circle   --cx 0 --cz 20  --cy 4  -h 30    --r 5 --block wool
shapegenerator.exe -s circle   --cx 0 --cz 40  --cy 4  -h 30    --r 5  -f

shapegenerator.exe -s sphere --x1 0 --z1 60  --y1 15  --r 5 --block glass


