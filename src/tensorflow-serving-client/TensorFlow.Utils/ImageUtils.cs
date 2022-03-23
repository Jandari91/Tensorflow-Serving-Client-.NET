using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace TensorFlow.Utils
{
    public class ImageUtils
    {
        public Image ResizeImage(Image image, int width, int height)
        {
            image.Mutate(x => x.Resize(width, height));
            return image;
        }

        public byte[] ImageToByteArray(Image image, IImageFormat format)
        {

            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, format);
                return memoryStream.ToArray();
            }
        }

        

        public float[][][] ConvertImageStreamToDimArrays(int numRows, int numCols, int numChannels, MemoryStream stream)
        {
            var imageMatrix = new float[numRows][][];
            for (int row = 0; row < numRows; row++)
            {
                imageMatrix[row] = new float[numCols][];
                for (int col = 0; col < numCols; col++)
                {
                    imageMatrix[row][col] = new float[numChannels];
                    for (int channel = 0; channel < numChannels; channel++)
                    {
                        imageMatrix[row][col][channel] = stream.ReadByte();
                    }
                }
            }
            return imageMatrix;
        }
        

        public (Image, IImageFormat) FromImageFile(string imagePath)
        {
            IImageFormat format = null;
            Image image = Image.Load(imagePath, out format);
            return (image, format);
        }
    }
}