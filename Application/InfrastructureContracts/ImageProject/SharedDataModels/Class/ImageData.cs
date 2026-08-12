namespace Application.InfrastructureContracts.ImageProject.SharedDataModels.Class
{
    public class ImageData<TColorDepth> : ImageInfo where TColorDepth : struct, IConvertible
    {

        public TColorDepth[,,] Data { get; init; }




        public ImageData() { }


        public ImageData(ImageInfo imageInfo) : base(imageInfo)
        { 
        }


        public ImageData(ImageInfo imageInfo, TColorDepth[,,] data) : base(imageInfo)
        {
            Data = data;
        }


    }
}
