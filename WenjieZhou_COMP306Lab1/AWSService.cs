// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: September 18, 2024


using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;


namespace WenjieZhou_COMP306Lab1
{
    public class AWSService
    {
        // ObservableCollection for storing Buckets and Objects
        public ObservableCollection<S3Bucket> BucketList { get; private set; } = new ObservableCollection<S3Bucket>();
        public ObservableCollection<S3Object> ObjectList { get; private set; } = new ObservableCollection<S3Object>();

        public AWSService()
        {
        }

        // Get Bucket List and update the bucket ObservableCollection
        public async Task GetBucketListAsync()
        {
            try
            {
                // Clear the existing list before fetching the new list
                BucketList.Clear();

                // Fetch the list of buckets and add them to the bucket ObservableCollection
                ListBucketsResponse response = await Helper.s3Client.ListBucketsAsync();
                foreach (S3Bucket bucket in response.Buckets)
                {
                    BucketList.Add(bucket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Create new Bucket and update the bucket ObservableCollection
        public async Task CreateBucketAsync(string bucketName)
        {
            try
            {
                var request = new PutBucketRequest { BucketName = bucketName };
                await Helper.s3Client.PutBucketAsync(request);
                BucketList.Add(new S3Bucket { BucketName = bucketName, CreationDate = DateTime.Now });
                MessageBox.Show("Bucket created successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    MessageBox.Show("Bucket name already exists. Please choose a different name.");
                }
                else
                {
                    MessageBox.Show($"Error creating bucket: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        // Delete an existing Bucket and update the bucket ObservableCollection
        public async Task DeleteBucketAsync(string bucketName)
        {
            try
            {
                // Check if the bucket is empty
                var listObjectsResponse = await Helper.s3Client.ListObjectsAsync(bucketName);
                if (listObjectsResponse.S3Objects.Count > 0)
                {
                    // Show confirmation dialog
                    var result = MessageBox.Show(
                        "The bucket is not empty. Do you want to empty it before deletion?",
                        "Confirm Deletion",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    // If the user chooses 'No', exit the method
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    // Proceed to delete objects in the bucket if the user chooses 'Yes'
                    var deleteObjectsRequest = new DeleteObjectsRequest { BucketName = bucketName };
                    foreach (var obj in listObjectsResponse.S3Objects)
                    {
                        deleteObjectsRequest.Objects.Add(new KeyVersion { Key = obj.Key });
                    }
                    await Helper.s3Client.DeleteObjectsAsync(deleteObjectsRequest);
                }

                // Delete the bucket when it is empty
                await Helper.s3Client.DeleteBucketAsync(bucketName);

                // Remove the bucket from the bucket ObservableCollection
                var bucketToRemove = BucketList.FirstOrDefault(b => b.BucketName == bucketName);
                if (bucketToRemove != null)
                {
                    BucketList.Remove(bucketToRemove);
                }

                MessageBox.Show("Bucket deleted successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("AWS S3 error occurred while deleting the bucket: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the bucket: " + ex.Message, ex);
            }
        }

        // Get Object List and update the object ObservableCollection
        public async Task GetObjectsAsync(string bucketName)
        {
            try
            {
                // Clear the existing list before fetching the new list
                ObjectList.Clear();

                // Fetch the list of objects and add them to the object ObservableCollection
                var listObjectsResponse = await Helper.s3Client.ListObjectsAsync(bucketName);
                foreach (var s3Object in listObjectsResponse.S3Objects)
                {
                    // Skip the folder objects
                    var fileName = System.IO.Path.GetFileName(s3Object.Key);
                    if (string.IsNullOrEmpty(fileName))
                        continue;

                    // Add the valid object to the object ObservableCollection
                    ObjectList.Add(new S3Object
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        Size = s3Object.Size
                    });
                }
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("AWS S3 error occurred while retrieving objects: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving objects: " + ex.Message, ex);
            }
        }

        // Upload Object and update the object ObservableCollection
        public async Task UploadObjectAsync(string bucketName, string key, string filePath)
        {
            try
            {
                var request = new PutObjectRequest { BucketName = bucketName, Key = key, FilePath = filePath };
                await Helper.s3Client.PutObjectAsync(request);

                // Get the file size
                var fileInfo = new FileInfo(filePath);
                long fileSize = fileInfo.Length;

                // Add the new object to the ObservableCollection
                ObjectList.Add(new S3Object
                {
                    BucketName = bucketName,
                    Key = key,
                    Size = fileSize
                });

                MessageBox.Show("File uploaded successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("AWS S3 error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("General error: " + ex.Message, ex);
            }
        }

        // Download Object to user-specified location
        public async Task DownloadObjectAsync(string bucketName, string key, string filePath)
        {
            try
            {
                var response = await Helper.s3Client.GetObjectAsync(bucketName, key);
                using (var responseStream = response.ResponseStream)
                {
                    using (var fileStream = System.IO.File.Create(filePath))
                    {
                        await responseStream.CopyToAsync(fileStream);
                    }
                }

                MessageBox.Show("File downloaded successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("AWS S3 error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("General error: " + ex.Message, ex);
            }
        }
    }
}