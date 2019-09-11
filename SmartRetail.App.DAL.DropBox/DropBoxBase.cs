using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.DropBox
{
    public class DropBoxBase : IPictureWareHouse
    {
        #region Variables
        private DropboxClient DBClient;
        private ListFolderArg DBFolders;
        private string oauth2State;
        private const string RedirectUri = "https://localhost/authorize"; // Same as we have configured Under [Application] -> settings -> redirect URIs.  
        #endregion

        #region Constructor
        
        public DropBoxBase(string ApiKey, string ApiSecret, string ApplicationName = "SmartRetailApp")
        {
            try
            {
                AppKey = ApiKey;
                AppSecret = ApiSecret;
                AppName = ApplicationName;
                AccessTocken = "-BQHXZ1VM7AAAAAAAAAAPo42Anr-YKilovBjV2NRiz8M-Fh4rEo2zEfZLVczCiZU";
                GeneratedAuthenticationURL();
                GenerateAccessToken();
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        #endregion
        
        #region Properties
        public string AppName
        {
            get;
            private set;
        }
        public string AuthenticationURL
        {
            get;
            private set;
        }
        public string AppKey
        {
            get;
            private set;
        }

        public string AppSecret
        {
            get;
            private set;
        }

        public string AccessTocken
        {
            get;
            private set;
        }
        public string Uid
        {
            get;
            private set;
        }
        #endregion

        #region UserDefined Methods

        /// <summary>  
        /// This method is to generate Authentication URL to redirect user for login process in Dropbox.  
        /// </summary>  
        /// <returns></returns>  
        public string GeneratedAuthenticationURL()
        {
            try
            {
                this.oauth2State = Guid.NewGuid().ToString("N");
                Uri authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, AppKey, RedirectUri, state: oauth2State);
                AuthenticationURL = authorizeUri.AbsoluteUri.ToString();
                return authorizeUri.AbsoluteUri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>  
        /// This method is to generate Access Token required to access dropbox outside of the environment (in ANy application).  
        /// </summary>  
        /// <returns></returns>  
        public string GenerateAccessToken()
        {
            try
            {
                string _strAccessToken = string.Empty;
                //this.AccessTocken = accessTocken;
                if (CanAuthenticate())
                {
                    if (string.IsNullOrEmpty(AuthenticationURL))
                    {
                        throw new Exception("AuthenticationURL is not generated !");
                    }

                    DropboxClientConfig CC = new DropboxClientConfig(AppName, 1);
                    HttpClient HTC = new HttpClient();
                    HTC.Timeout = TimeSpan.FromMinutes(10); // set timeout for each ghttp request to Dropbox API.  
                    CC.HttpClient = HTC;
                    DBClient = new DropboxClient(AccessTocken, CC);
                }

                return AccessTocken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        /// <summary>  
        /// Method to delete file/folder from Dropbox  
        /// </summary>  
        /// <param name="path">path of file.folder to delete</param>  
        /// <returns></returns>  
        public async Task<bool> Delete(string path)
        {
            try
            {
                if (AccessTocken == null)
                {
                    throw new Exception("AccessToken not generated !");
                }
                if (AuthenticationURL == null)
                {
                    throw new Exception("AuthenticationURI not generated !");
                }

                var folders = await DBClient.Files.DeleteV2Async(path);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Что-то не так: " + ex.Message);
            }
        }

        /// <summary>  
        /// Method to upload files on Dropbox  
        /// </summary>  
        /// <param name="UploadfolderPath"> Dropbox path where we want to upload files</param>  
        /// <param name="UploadfileName"> File name to be created in Dropbox</param>  
        /// <param name="SourceFilePath"> Local file path which we want to upload</param>  
        /// <returns></returns>  
        public async Task<string> Upload(string UploadfolderPath, string UploadfileName, string SourceFilePath)
        {
            try
            {
                using (var fileStream = File.Open(SourceFilePath, FileMode.Open))
                {
                    var s = await DBClient.Files.UploadAsync(UploadfolderPath + "/" + UploadfileName, body: fileStream);
                    var result = await DBClient.Sharing.CreateSharedLinkWithSettingsAsync(UploadfolderPath + "/" + UploadfileName);
                    return result.Url;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Что-то пошло не так..." + ex.Message);
            }
        }

        public async Task<string> Upload(MemoryStream content, string path)
        {
            
            var s = await DBClient.Files.UploadAsync(path, body:content);
            var result = await DBClient.Sharing.CreateSharedLinkWithSettingsAsync(path);
            return result.Url;
        }

        public async Task<string> GetFileWithSharedLink(string sharedLink)
        {
            try
            {
                var result = await DBClient.Sharing.GetSharedLinkFileAsync(sharedLink);
                return result.Response.PathLower;
            }
            catch (Exception)
            {
                throw new Exception("Нет такого файла!");
            }
        }


        #endregion
        
        #region Validation Methods
        /// <summary>  
        /// Validation method to verify that AppKey and AppSecret is not blank.  
        /// Mendatory to complete Authentication process successfully.  
        /// </summary>  
        /// <returns></returns>  
        public bool CanAuthenticate()
        {
            try
            {
                if (AppKey == null)
                {
                    throw new ArgumentNullException("AppKey");
                }
                if (AppSecret == null)
                {
                    throw new ArgumentNullException("AppSecret");
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
    }
}
