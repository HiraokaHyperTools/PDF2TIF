set exe=%~dp0\..\bin\DEBUG\PDF2TIF.exe
echo y|del "%~dp0\out\*.*"
set PDF2TIF_BPP=
set PDF2TIF_DPI=
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages.tif"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages@{}.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages.jpg"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages@{}.jpg"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages.ppm"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages@{}.ppm"

set PDF2TIF_BPP=1
set PDF2TIF_DPI=
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-1bpp.tif"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-1bpp.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-1bpp@{}.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-1bpp.ppm"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-1bpp-{}.pbm"

set PDF2TIF_BPP=8
set PDF2TIF_DPI=
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-8bpp.tif"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-8bpp.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-8bpp@{}.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-8bpp.ppm"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-8bpp-{}.pgm"

set PDF2TIF_BPP=600
set PDF2TIF_DPI=
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi.tif"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi@{}.png"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi.jpg"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi@{}.jpg"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi.ppm"
"%exe%" "%~dp0\in\3pages.pdf" %~dp0\out\3pages-600dpi@{}.ppm"
