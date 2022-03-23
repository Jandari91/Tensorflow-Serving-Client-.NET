
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;

namespace TensorFlow.Utils
{
    public class TensorBuilder
    {
        public static TensorProto CreateTensorFromImage(Image image, IImageFormat format, float revertsBits = 1.0f)
        {
			int width = Convert.ToInt32(image.Width);
			int height = Convert.ToInt32(image.Height);
			var imageUtils = new ImageUtils();


			var bytes = imageUtils.ImageToByteArray(image, format);

			var memoryStream = new MemoryStream();
			memoryStream.Write(bytes);

			var dimArray = imageUtils.ConvertImageStreamToDimArrays(width, height, 3, memoryStream);
            return CreateTensorFromImage(dimArray, revertsBits, dimArray.Length, dimArray[0].Length, dimArray[0][0].Length);
        }

		public static TensorProto CreateTensorFromImage(float[][][] dimArray, float revertsBits, int height, int width, int channels)
		{
			var imageFeatureShape = new TensorShapeProto();

			imageFeatureShape.Dim.Add(new TensorShapeProto.Types.Dim() { Size = 1 });
			imageFeatureShape.Dim.Add(new TensorShapeProto.Types.Dim() { Size = height });
			imageFeatureShape.Dim.Add(new TensorShapeProto.Types.Dim() { Size = width });
			imageFeatureShape.Dim.Add(new TensorShapeProto.Types.Dim() { Size = channels });

			var imageTensorBuilder = new TensorProto();
			imageTensorBuilder.Dtype = DataType.DtFloat;
			imageTensorBuilder.TensorShape = imageFeatureShape;

			for (int i = 0; i < height; ++i)
			{
				for (int j = 0; j < width; ++j)
				{
					for (int c = 0; c < channels; c++)
					{
						imageTensorBuilder.FloatVal.Add(dimArray[i][j][c] / 255.0F);
					}
				}
			}

			return imageTensorBuilder;
		}
	}
}
