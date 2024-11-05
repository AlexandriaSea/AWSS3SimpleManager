// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: September 18, 2024


using System.Windows;


namespace WenjieZhou_COMP306Lab1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handlers for the bucket operation button
        private void BucketLevelOperations_Click(object sender, RoutedEventArgs e)
        {
            var bucketWindow = new BucketOperations();
            bucketWindow.Show();
            this.Hide();
        }

        // Event handlers for the object operation button
        private void ObjectLevelOperations_Click(object sender, RoutedEventArgs e)
        {
            var objectWindow = new ObjectOperations();
            objectWindow.Show();
            this.Hide();
        }

        // Event handlers for the exit button
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Dispose the s3 client and exit the application
            Helper.s3Client?.Dispose();
            Application.Current.Shutdown();
        }
    }
}