// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: September 18, 2024


using Amazon.S3;
using System.Configuration;


namespace WenjieZhou_COMP306Lab1
{
    public static class Helper
    {
        public readonly static IAmazonS3 s3Client;

        static Helper()
        {
            s3Client = GetAmazonS3Client();
        }

        private static IAmazonS3 GetAmazonS3Client()
        {
            string awsAccessId = ConfigurationManager.AppSettings["accessKeyId"];
            string awsSecretKey = ConfigurationManager.AppSettings["secretAccessKey"];
            return new AmazonS3Client(awsAccessId, awsSecretKey, Amazon.RegionEndpoint.CACentral1);
        }
    }
}