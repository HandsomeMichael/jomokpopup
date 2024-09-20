using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

public class FullScreenImagePopUp : Form
{
    private static Random rand = new Random();
    private static readonly string[] imageFiles = { "apple.png", "banana.jpg", "orange.jpeg", "pineapple.jpeg" };
    private PictureBox pictureBox;

    public FullScreenImagePopUp()
    {
        // Set the form properties to full-screen
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.TopMost = true;

        // Create and configure the PictureBox to display the image
        pictureBox = new PictureBox();
        pictureBox.Dock = DockStyle.Fill;

        string randomImage = imageFiles[rand.Next(imageFiles.Length)];
        string resourceName = $"fruitspopup.{randomImage}"; // Replace 'YourNamespace' with your actual namespace

        try
        {
            // Print all resource names for debugging
            ListResourceNames();

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found", randomImage);
                }
                using (Image image = Image.FromStream(stream))
                {
                    pictureBox.Image = new Bitmap(image);
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            MessageBox.Show("Image file not found: " + randomImage + "\nError: " + ex.Message);
        }

        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        this.Controls.Add(pictureBox);

        // Disable user input temporarily
        this.Capture = true;
        this.Focus();

        // Timer to close the form after .5 seconds
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        timer.Interval = 500; // .5 seconds
        timer.Tick += (sender, e) => this.Close();
        timer.Start();
    }

    public static async Task ShowImageAsync()
    {
        while (true)
        {
            Application.Run(new FullScreenImagePopUp());

            // Wait for a random amount of time between 1 and 3 seconds
            await Task.Delay(rand.Next(1000, 3000));
        }
    }

    [STAThread]
    public static void Main()
    {
        Task.Run(() => ShowImageAsync());
        Application.Run(); // Keeps the application alive
    }

    private static void ListResourceNames()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        foreach (var resourceName in resourceNames)
        {
            Console.WriteLine(resourceName);
        }
    }
}
