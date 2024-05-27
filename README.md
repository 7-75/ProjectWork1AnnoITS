# This project's aim is to make an application capable of reading the text from an image and save it to file to retrieve later trough the same application.
### The original specifications for this project was to use TenguCV for OCR and add the capabilities for video as well as color recognition and Windows Form for the graphical interface.
### Unfortunately due to compabilities issues with Fedora Asahi Linux (aarch64) it is both not possible to build the app with those two technologies within the allotted time.

# I instead built the application with:
- Gtk, (Using Glade as a development tool for the UI).
- Gtk# (to bridge the gtk components over to C#).
- The tesseract CLI binary for image text detection.
- CliWrap to run the binary from within C#.

# The first thing I did was to make sure I had dotnet 6.0 installed

`# dnf install dotnet-6.0 (controllare)`

# Then I installed all the other tools such as Glade

`# dnf install glade`

# Then I installed the GTK# library from the dotnet-cli tool

`$ dotnet install GtkSharp (controllare)`

# And everything is set up to create a new project

`$ mkdir -p ˜/Projects/myapp`
`$ cd ˜/Projects/myapp`
`$ dotnet new gtkapp`

# Add the libraries needed for the developement
`dotnet add package CliWrap`
`dotnet add package GtkSharp`

# Now that the template is created and libraries are installed we can start our editor of choice and stard coding (My editor is VS Code)

`$ code`

## For what concerns the GUI we can edit the window.glade file in the project root folder, directly with the Glade editor

`Glade ˜/Projects/myapp/MainWindow.glade`
