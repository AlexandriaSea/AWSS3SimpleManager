// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: September 18, 2024


using Amazon.S3.Model;
using System.Windows;


namespace WenjieZhou_COMP306Lab1
{
    public partial class BucketOperations : Window
    {
        private AWSService _awsService;

        public BucketOperations()
        {
            InitializeComponent();

            // Initialize the AWSService and bind ObservableCollection to the DataGrid
            _awsService = new AWSService();
            BucketsDataGrid.ItemsSource = _awsService.BucketList;

            LoadBuckets();
        }

        // Event to show the explanation when the user focuses on the text box
        private void BucketNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BucketNamingRuleTextBlock.Text = "Bucket naming rules:\n" +
                "1. Bucket names must be unique across all AWS S3.\n" +
                "2. Must be between 3 and 63 characters long.\n" +
                "3. Only lowercase letters, numbers, hyphens, and periods are allowed.\n" +
                "4. Cannot begin with a period or a hyphen.";
            BucketNamingRuleTextBlock.Visibility = Visibility.Visible;
        }

        // Event to hide the explanation when the user leaves the text box
        private void BucketNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            BucketNamingRuleTextBlock.Visibility = Visibility.Collapsed;
        }

        // Load list of buckets
        private async void LoadBuckets()
        {
            await _awsService.GetBucketListAsync();
        }

        // Event handler for the Create Bucket button
        private async void CreateBucket_Click(object sender, RoutedEventArgs e)
        {
            var bucketName = BucketNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(bucketName))
            {
                await _awsService.CreateBucketAsync(bucketName);
                BucketNameTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Bucket name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for the Delete Bucket button
        private async void DeleteBucket_Click(object sender, RoutedEventArgs e)
        {
            var selectedBucket = BucketsDataGrid.SelectedItem as S3Bucket;
            if (selectedBucket != null)
            {
                await _awsService.DeleteBucketAsync(selectedBucket.BucketName);
            }
            else
            {
                MessageBox.Show("Please select a bucket to delete.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for the Back to Main button
        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}