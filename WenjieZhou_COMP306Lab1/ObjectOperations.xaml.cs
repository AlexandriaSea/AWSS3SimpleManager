// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: September 18, 2024


using Amazon.S3.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;


namespace WenjieZhou_COMP306Lab1
{
    public partial class ObjectOperations : Window
    {
        private AWSService _awsService;
        private string selectedFilePath;

        public ObjectOperations()
        {
            InitializeComponent();

            // Initialize AWSService and bind ObservableCollection to the ComboBox and DataGrid
            _awsService = new AWSService();
            BucketComboBox.ItemsSource = _awsService.BucketList;
            ObjectsDataGrid.ItemsSource = _awsService.ObjectList;

            LoadBuckets();
        }

        // Load list of buckets
        private async void LoadBuckets()
        {
            await _awsService.GetBucketListAsync();
        }

        // Load objects when a bucket is selected
        private async void LoadObjects(string bucketName)
        {
            await _awsService.GetObjectsAsync(bucketName);
        }

        // Handle bucket selection in ComboBox
        private void BucketComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBucket = BucketComboBox.SelectedItem as S3Bucket;
            if (selectedBucket != null)
            {
                LoadObjects(selectedBucket.BucketName);
            }
        }

        // Browse user's file system to select a file and display the file name in the text box
        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                ObjectTextBox.Text = System.IO.Path.GetFileName(selectedFilePath);
            }
        }

        // Upload the selected file to the selected bucket
        private async void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedBucket = BucketComboBox.SelectedItem as S3Bucket;
            if (selectedBucket != null && !string.IsNullOrEmpty(selectedFilePath))
            {
                var fileName = System.IO.Path.GetFileName(selectedFilePath);
                await _awsService.UploadObjectAsync(selectedBucket.BucketName, fileName, selectedFilePath);
                ObjectTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Please select a bucket and a file to upload.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Download the selected object from the S3 bucket to the user-specified location
        private async void DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedObject = ObjectsDataGrid.SelectedItem as S3Object;
            if (selectedObject != null)
            {
                // Use SaveFileDialog to select destination path for download
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = selectedObject.Key,
                    Title = "Save File",
                    Filter = "All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var destinationPath = saveFileDialog.FileName;
                    await _awsService.DownloadObjectAsync(selectedObject.BucketName, selectedObject.Key, destinationPath);
                }
            }
            else
            {
                MessageBox.Show("Please select an object to download.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
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