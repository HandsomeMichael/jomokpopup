using System.Media;
using System.Reflection;
using Timer = System.Windows.Forms.Timer;

public class FullScreenImagePopUp : Form
{
    private static readonly Random rand = new Random();
    private static readonly string[] imageFiles = { "apple.png", "banana.jpg", "orange.jpeg", "pineapple.jpeg" };
    private PictureBox pictureBox;
    public Timer timer = new();

    public FullScreenImagePopUp()
    {
        // Play sound when the popup appears
        PlaySound();  // Call this at the start of the popup

        // Set the form properties to full-screen because why not?
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.TopMost = true;

        // Create and configure the PictureBox to display the image
        pictureBox = new PictureBox();
        pictureBox.Dock = DockStyle.Fill;

        // Replace 'jomokpopup' with your actual namespace
        string randomImage = imageFiles[rand.Next(imageFiles.Length)];
        string resourceName = $"jomokpopup.assets.{randomImage}";

        try
        {
            // Print all resource names for debugging, just in case.
            ListResourceNames();

            using Stream stream = GetStreamFile(resourceName);
            using Image image = Image.FromStream(stream);
            pictureBox.Image = new Bitmap(image);
        }
        catch (FileNotFoundException ex)
        {
            MessageBox.Show("Image file not found: " + randomImage + "\nError: " + ex.Message);
        }

        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        this.Controls.Add(pictureBox);

        // Disable user input temporarily, just because we can.
        this.Capture = true;
        this.Focus();

        // Give users at least half a second to admire your fruit pictures before we ninja it away.
        timer.Interval = 500; // Half a second sounds more reasonable
        timer.Tick += NewTick;

        timer.Start();
    }

    private void NewTick(object? sender, EventArgs e)
    {
        // Slow down cowboy, give it a fade out instead of just vanishing.
        this.Opacity -= 0.1; 

        if (this.Opacity <= 0)
        {
            timer.Dispose();
            this.Close();
        }
    }

    private void PlaySound()
    {
        try
        {
            // Load the embedded WAV resource
            var assembly = Assembly.GetExecutingAssembly();

            using Stream stream = GetStreamFile("jomokpopup.sounds.bom.wav");
            if (stream == null) return;

            // Use SoundPlayer to play the embedded WAV file
            SoundPlayer player = new SoundPlayer(stream);
            player.Play();  // Play asynchronously
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error playing sound: " + ex.Message);
        }
    }

    public Stream GetStreamFile(string resourceName)
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException("Resource not found", resourceName);
    }
    

    public static async Task ShowImageAsync()
    {
        while (true)
        {
            var app = new FullScreenImagePopUp();
            Application.Run(app);

            // Wait for a random amount of time between 1 and 3 seconds
            await Task.Delay(rand.Next(1000, 3000));
        }
    }

    

    [STAThread]
    public static void Main()
    {
        Task.Run(() => ShowImageAsync());
        Application.Run(); // Keeps the application alive and looping
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
