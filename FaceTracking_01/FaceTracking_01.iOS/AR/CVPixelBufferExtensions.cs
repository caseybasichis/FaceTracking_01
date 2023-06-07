using CoreVideo;
using CoreImage;
using Foundation;
using CoreGraphics;

namespace FaceTracking_01.iOS.AR
{
    public static class CVPixelBufferExtensions
    {
        public static void YCbCrToRGBAPixelBuffer(this CVPixelBuffer pixelBuffer, out CVPixelBuffer rgbaPixelBuffer)
        {
            // Create a CIImage from the YCbCr pixel buffer
            var ciImage = new CIImage(pixelBuffer);

            // Create a CIContext
            var ciContext = CIContext.FromOptions(null);

            // Render the CIImage to a new RGBA pixel buffer
            rgbaPixelBuffer = ciContext.CreateCGImage(ciImage, ciImage.Extent).ToPixelBuffer();
        }

        public static CVPixelBuffer ToPixelBuffer(this CoreGraphics.CGImage cgImage)
        {
            var attrs = new CIContextOptions();
            var ciContext = CIContext.FromOptions(attrs);
            var ciImage = new CIImage(cgImage);
            var size = ciImage.Extent.Size;
            var pixelBuffer = new CVPixelBuffer((int)size.Width, (int)size.Height, CVPixelFormatType.CV32BGRA);

            ciContext.Render(ciImage, pixelBuffer);

            return pixelBuffer;
        }
    }
}