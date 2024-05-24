using System;
using System.IO;
using Gdk;
using Gtk;
using System.Text.Json;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using System.Threading.Tasks;

namespace Prove
{
    class MainWindow : Window
    {
        //coupling gtk objects with the fields
        [UI] private readonly Button Btn_Salva = null;
        [UI] private readonly Button Btn_SalvaCome = null;
        [UI] private readonly Button Btn_Estrai = null;
        [UI] private readonly FileChooserButton FCBtn_Main = null;
        [UI] private readonly Image Dis_Image = null;
        [UI] private readonly ListBox Lbx_Registro = null;
        [UI] private readonly TextView TxV_Main = null;
        [UI] private readonly Button Btn_ApriRegistro = null;

        [UI] private readonly Window Wdw_Registro = null;

        /// <summary>
        /// The main capture object.
        /// </summary>
        Capture mainCapture;

        /// <summary>
        /// The main Pixbuf object.
        /// </summary>
        private Pixbuf mainPixbuf;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        /// <param name="builder"></param>
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            Btn_ApriRegistro.Clicked += OnButtonApriRegistroClicked;
            FCBtn_Main.FileSet += OnFileChooserButtonFileSet;
            Btn_Estrai.Clicked += OnButtonEstraiClicked;
            DeleteEvent += Window_DeleteEvent;
            Wdw_Registro.DeleteEvent += Registro_DeleteEvent;
            Btn_Salva.Clicked += OnButtonSalvaClickedAsync;
            Btn_SalvaCome.Clicked += OnButtonSalvaComeClickedAsync;
        }


        /// <summary>
        /// Event handler for the file chooser button's file set event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// 
        private void OnFileChooserButtonFileSet(object sender, EventArgs e)
        {
            if (sender is FileChooserButton FCBtn_Main)
            {
                string filename = FCBtn_Main.Filename;

                // Load the image
                mainPixbuf = new (filename);
                ScaleAndDisplayImage();

                Btn_Estrai.SetProperty("sensitive", new GLib.Value(true));
            }
        }

        /// <summary>
        /// Event handler for the "Estrai" button click event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// 
        private async void OnButtonEstraiClicked(object sender, EventArgs e)
        {
            if (mainPixbuf == null)
            {
                return;
            }

            Btn_Estrai.SetProperty("sensitive", new GLib.Value(false));
            TxV_Main.Buffer.Text = "Estrazione in corso...";

            // Create a new Capture object
            mainCapture = await FileCapture.Execute(FCBtn_Main.Filename);

            // Get the output from the Capture object
            string output = mainCapture.Content;

            // Display the output in the text view
            TxV_Main.Buffer.Text = output;

            Btn_Estrai.SetProperty("sensitive", new GLib.Value(true));
            EnableSaveButtons();

        }

        /// <summary>
        /// Scales the image to fit the window and displays it.
        /// </summary>
        private void ScaleAndDisplayImage()
        {
            int allocatedHeight = Dis_Image.AllocatedHeight;
            int allocatedWidth = Dis_Image.AllocatedWidth;

            int pixbufHeight = mainPixbuf.Height;
            int pixbufWidth = mainPixbuf.Width;


            // calculate the aspect ratio of the image
            double aspectRatio = (double)pixbufWidth / pixbufHeight;

            // calculate the aspect ratio of the frame
            double frameAspectRatio = (double)allocatedWidth / allocatedHeight;

            // calculate the width and height of the frame
            int frameWidth;
            int frameHeight;

            // if the image is wider than the frame
            if (aspectRatio > frameAspectRatio)
            {
                // calculate the width of the frame
                frameWidth = allocatedWidth;

                // calculate the height of the frame
                frameHeight = (int)(allocatedWidth / aspectRatio);
            }
            else
            {
                // calculate the width of the frame
                frameWidth = (int)(allocatedHeight * aspectRatio);

                // calculate the height of the frame
                frameHeight = allocatedHeight;
            }


            // Create a new Pixbuf with the desired size
            Pixbuf newPixbuf = new (mainPixbuf.Colorspace, mainPixbuf.HasAlpha, mainPixbuf.BitsPerSample, frameWidth, frameHeight);

            // Scale the original image into the new Pixbuf
            mainPixbuf.Scale(newPixbuf, 0, 0, frameWidth, frameHeight, 0, 0, (double)frameWidth / mainPixbuf.Width, (double)frameHeight / mainPixbuf.Height, InterpType.Bilinear);

            // Display the scaled image
            Dis_Image.Pixbuf = newPixbuf;

        }

        /// <summary>
        /// Event handler for the "Apri Registro" button click event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        ///

        private void OnButtonApriRegistroClicked(object sender, EventArgs e)
        {
            Wdw_Registro.Show();
            FillListBox();
        }

        /// <summary>
        /// Fills the listbox with the contents of the "scans" folder.
        /// </summary>
        private void FillListBox()
        {
            string[] files = Directory.GetFiles("scans");

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                
                //Deserialize the json string into a Capture object
                Capture capture = JsonSerializer.Deserialize<Capture>(json);

                //Get the content of the Capture object and put it in the listbox
                string content = capture.Content;

                //Get the path of the Capture object    
                string path = capture.Path;

                //Create a new vertical box to hold two boxes
                Box mainBox = new(Orientation.Horizontal, 0);
                Box leftBox = new(Orientation.Vertical, 0);
                Box rightBox = new(Orientation.Vertical, 0);

                //Create a new label to hold the name of the file of the capture
                Label labelPath = new(path)
                {
                    Wrap = true,
                    Justify = Justification.Left,
                    LineWrapMode = Pango.WrapMode.WordChar,
                    Margin = 5,
                    //make bold
                    Markup = $"<b>{path}</b>"
                };


                //Create a label to hold the file currently being added
                Label labelFile = new(file.ToString())
                {
                    Wrap = true,
                    Justify = Justification.Left,
                    LineWrapMode = Pango.WrapMode.WordChar,
                    HeightRequest = 10,
                    //make bold
                    Markup = $"<b>{file}</b>"

                };

                //Create a new label to hold the content
                Label labelContent = new(content)
                {
                    Wrap = true,
                    Justify = Justification.Left,
                    LineWrapMode = Pango.WrapMode.WordChar,
                    HeightRequest = 10
                };


                // Load the original Pixbuf
                Pixbuf originalPixbuf = new(path);

                // Scale the Pixbuf to the desired size
                int desiredWidth = 500;
                int desiredHeight = 500;
                Pixbuf scaledPixbuf = originalPixbuf.ScaleSimple(desiredWidth, desiredHeight, InterpType.Bilinear);

                // Dispose of the original Pixbuf
                originalPixbuf.Dispose();

                Image image = new(scaledPixbuf)
                {
                    Margin = 5,
                    Halign = Align.Fill,
                    Valign = Align.Fill,
                    Expand = true,
                };

                leftBox.Add(labelPath);
                leftBox.Add(image);
                rightBox.Add(labelFile);
                rightBox.Add(labelContent);

                mainBox.Add(leftBox);
                mainBox.Add(rightBox);
                
                //Dispose of the old pixbuf
                scaledPixbuf.Dispose();

                //Dispose of the labels
                labelContent.Dispose();
                labelFile.Dispose();
                labelPath.Dispose();

                //Create a new listbox row
                ListBoxRow row = new()
                {
                    mainBox,

                };

                //Add the row to the listbox
                Lbx_Registro.Add(row);

            }


                //Show the listbox
                Lbx_Registro.ShowAll();
        }


        /// <summary>
        /// Event handler for the "Salva" button click event.
        /// Saves the captured image asynchronously as a JSON file.
        /// </summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonSalvaClickedAsync(object sender, EventArgs e)
        {
            //check if capture and pixbuf are not null
            if (mainPixbuf == null || mainCapture == null)
            {
                return;
            }

            await Task.Run(async () => await SaveTask.SaveAsync(mainCapture));
            
            // Create a new message dialog
            Notify("File salvato con successo");

        }
        

        /// <summary>
        /// Event handler for the "Salva Come" button click event.
        /// Saves the captured image asynchronously as a JSON file.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// 
        private async void OnButtonSalvaComeClickedAsync(object sender, EventArgs e)
        {
            //check if capture and pixbuf are not null
            if (mainPixbuf == null || mainCapture == null)
            {
                return;
            }


            FileChooserDialog fileChooser = new("Salva immagine", this, FileChooserAction.Save, "Annulla", ResponseType.Cancel, "Salva", ResponseType.Accept);
           
            //set json file filter
            FileFilter filter = new();
            filter.AddPattern("*.json");
            filter.Name = "JSON files";
            fileChooser.AddFilter(filter);

            //add json as the default file extension
            fileChooser.DefaultResponse = ResponseType.Accept;

            //add default file name
            fileChooser.CurrentName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".json";

            //set the file chooser to save mode
            fileChooser.SelectMultiple = false;

            //add default folder
            fileChooser.SetCurrentFolder("scans");

            //display the dialog
            ResponseType response = (ResponseType)fileChooser.Run();
            
            //if the user clicked the save button
            if (response == ResponseType.Accept)
            {
                //get the file name from the dialog
                string filename = fileChooser.Filename;
                //save the image asynchronously
                await Task.Run(async () => await SaveTask.SaveAsync($"scans/{filename}.json", mainCapture));
  
            }

            if (response == ResponseType.Cancel)
            {
                //close the dialog
                fileChooser.Destroy();
                return;
            }

            // Create a new message dialog
            Notify("File salvato con successo");


            //close the dialog
            fileChooser.Destroy();
        
        }

        /// <summary>
        /// Enables the save buttons.
        /// </summary>
        /// 
        private void EnableSaveButtons()
        {
            Btn_Salva.SetProperty("sensitive", new GLib.Value(true));
            Btn_SalvaCome.SetProperty("sensitive", new GLib.Value(true));
        }

        /// <summary>
        /// Displays a notification message dialog.
        /// </summary>
        /// <param name="message">The message to be displayed in the dialog.</param>
        /// 
        private new void Notify(string message)
        {
            // Create a new message dialog
            MessageDialog dialog = new(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, message);
            dialog.Run();

            // Close the dialog when ok is pressed
            dialog.Destroy();
        }


        /// <summary>
        /// Event handler for the window delete event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="a">The event arguments.</param>
        /// 
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
        
        
        /// <summary>
        /// Event handler for the DeleteEvent event of the Registro window.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="a">The event arguments.</param>
        /// 
        private void Registro_DeleteEvent(object sender, DeleteEventArgs a)
        {
            //hide the window
            Wdw_Registro.Hide();

            //stop the event
            a.RetVal = true;
        }


    }
}
