
using Grpc.Core;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using System.Drawing;
using Tensorflow.Serving;
using TensorFlow.Utils;
using TensorflowServingClient;
using System.Reflection;

var imageFile = @"D:\Workspace\Project\Tensorflow-Serving-Client-.NET\src\tensorflow-serving-client\tensorflow-serving-client\test_image\1_1.jpg";
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


var imageUtils = new ImageUtils();
var (image, format) = imageUtils.FromImageFile(imageFile);
var resizeImage = imageUtils.ResizeImage(image, 480, 480);



var request = new PredictRequest()
{
    ModelSpec = new ModelSpec() { Name = "test-model", SignatureName = "serving_default" }
};

var tensor = TensorBuilder.CreateTensorFromImage(resizeImage, format, 1.0f);
request.Inputs.Add("input_1", tensor);

var predictResponse = client.Predict(request);


// Get predict output
var maxValue = predictResponse.Outputs["predictions"].FloatVal.Max();
//Get index of predicted value
var predictedValue = predictResponse.Outputs["predictions"].FloatVal.IndexOf(maxValue);


Console.WriteLine($"Result value: {predictedValue}, probability: {maxValue}");
Console.WriteLine($"All values: {predictResponse.Outputs["predictions"].FloatVal}");
Console.WriteLine("");
