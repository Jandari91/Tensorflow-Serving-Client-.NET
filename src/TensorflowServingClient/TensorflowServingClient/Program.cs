using Tensorflow.Serving;
using Tensorflow_Serving.Utils;
using TensorflowServingProto;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TensorflowServingClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var imageFile = @"D:\Workspace\Project\Tensorflow-Serving-Client-.NET\src\tensorflow-serving-client\tensorflow-serving-client\test_image\5547758_eea9edfd54_n.jpg";

            var localServer = "127.0.0.1:8500";

            var channel = new Channel(localServer, ChannelCredentials.Insecure,
                            new List<Grpc.Core.ChannelOption> {
                    new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                    new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue)
                            });

            var client = new PredictionService.PredictionServiceClient(channel);

            //Check available model
            var responce = client.GetModelMetadata(new GetModelMetadataRequest()
            {
                ModelSpec = new ModelSpec() { Name = "test-model" },
                MetadataField = { "signature_def" }
            });

            Console.WriteLine($"Model Available: {responce.ModelSpec.Name} Ver.{responce.ModelSpec.Version}");


            var request = new PredictRequest()
            {
                ModelSpec = new ModelSpec() { Name = "test-model", SignatureName = "serving_default" }
            };


            var bmp = ImageUtils.ResizeImage(Image.FromFile(imageFile), 480, 480, 392, 166);

            MemoryStream ms = new MemoryStream();   // 초기화
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);


            var tensor = TensorBuilder.CreateTensorFromImage(ms, 1.0f);


            request.Inputs.Add("input_1", tensor);

            var predictResponse = client.Predict(request);


            // Get predict output
            var maxValue = predictResponse.Outputs["predictions"].FloatVal.Max();
            //Get index of predicted value
            var predictedValue = predictResponse.Outputs["predictions"].FloatVal.IndexOf(maxValue);


            Console.WriteLine($"Result value: {predictedValue}, probability: {maxValue}");
            Console.WriteLine($"All values: {predictResponse.Outputs["predictions"].FloatVal}");
            Console.WriteLine("");
        }
    }
}
