namespace GDC.EventHost.App.Components
{
    public class UploadFileInfo
    {

        public bool UploadWasSuccessful()
        {
            return !string.IsNullOrEmpty(PathToFile) && string.IsNullOrEmpty(ErrorMessages);
        }

        public string PathToFile { get; set; }

        public string ErrorMessages { get; set; }

    } 
}
