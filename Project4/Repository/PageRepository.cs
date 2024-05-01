
using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using Project4.Data;
using Project4.DTO;
using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Amazon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project4.Repository;

namespace Project4.Repository
{
    public interface IPageRepository : IBaseRepository<Pagee> 
    {
        Task<List<ContentDTO>> GetDetailContents();
        Task<String> GetS3FilesImage(String image);
        Task<String> WritingAnObjectAsync([FromForm] IFormFile file);
        Task<List<Pagee>> GetPagesByContent(string content);
        Task<Pagee> CreatePage(Pagee page);

    }

    public class PageRepository : BaseRepository<Pagee>, IPageRepository
    {
        public PageRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {

        }

        public async Task<List<ContentDTO>> GetDetailContents()
        {
            var query = from p in _context.Pagees.AsQueryable()
                        join c in _context.Chapters.AsQueryable() on p.ChapterId equals c.Id
                        group p by new
                        {
                            c.Id,
                            ContentName = p.Content,
                            ChapterName = c.SubName
                        } into grouped
                        select new ContentDTO
                        {
                            Id = grouped.Key.Id,
                            ContentName = grouped.Key.ContentName,
                            ChapterName = grouped.Key.ChapterName,
                        };
            return await query.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<String> GetS3FilesImage(String image)
        {
            string keyName = image;
            string bucketName = "upload-imagese";
            string accessKeyID = "AKIAZQ3DTKSFR45YTI7U";
            string secretAccessKeyID = "GHhiMS9Hew7OuOoBLYTS+atfohHHEhvWDxApn0e3";
            BasicAWSCredentials creds = new BasicAWSCredentials(accessKeyID, secretAccessKeyID);

            AmazonS3Config config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.APSoutheast2
            };

            AmazonS3Client client = new AmazonS3Client(creds, config);


            GetPreSignedUrlRequest preSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = keyName,
                Expires = DateTime.UtcNow.AddHours(7)
            };

            string preSignedUrl = client.GetPreSignedURL(preSignedUrlRequest);

            return preSignedUrl;
        }

        public async Task<String> WritingAnObjectAsync([FromForm] IFormFile file)
        {
            string bucketName = "upload-imagese";
            string accessKeyID = "AKIAZQ3DTKSFR45YTI7U";
            string secretAccessKeyID = "GHhiMS9Hew7OuOoBLYTS+atfohHHEhvWDxApn0e3";
            if (file == null || file.ToString()!.Length == 0)
            {
                return "Invalid request or empty image";
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var putRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = file.FileName,
                        InputStream = memoryStream,
                        ContentType = file.ContentType
                    };


                    BasicAWSCredentials creds = new BasicAWSCredentials(accessKeyID, secretAccessKeyID);

                    AmazonS3Config config = new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.APSoutheast2
                    };

                    AmazonS3Client client = new AmazonS3Client(creds, config);
                    var putResponse = await client.PutObjectAsync(putRequest);
                    Console.WriteLine($"putResponse putResponse: {0}", putResponse);

                    // Determine the encryption state of an object.
                    GetObjectMetadataRequest metadataRequest = new GetObjectMetadataRequest
                    {
                        BucketName = bucketName,
                        Key = file.FileName,
                    };
                    GetObjectMetadataResponse response = await client.GetObjectMetadataAsync(metadataRequest);
                    ServerSideEncryptionMethod objectEncryption = response.ServerSideEncryptionMethod;

                    Console.WriteLine($"Encryption method used: {0}", objectEncryption.ToString());
                    return file.FileName;
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error: '{ex.Message}' when writing an object");
                return null;
            }
        }

        public async Task<List<Pagee>> GetPagesByContent(string content)
        {
            var query = from p in _context.Pagees.AsQueryable()
                        where p.Content.ToLower().Contains(content.ToLower())

                        select p;

            return await query.ToListAsync();
        }

        public async Task<Pagee> CreatePage(Pagee page)
        {
            _context.Pagees.Add(page);
            await _context.SaveChangesAsync();
            return null;
        }

    }
}
