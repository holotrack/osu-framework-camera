# Cameras for osu!framework

Adds camera device support for [osu!framework](https://github.com/ppy/osu-framework) using [OpenCVSharp](https://github.com/shimat/opencvsharp). Currently supports Windows and Linux only.
This library is available via the GitHub Package Registry and NuGet.

### Additions
This library adds new drawable components to the framework:
- `DrawableCameraDevice`
	- A drawable capable of drawing video out from a phsical camera source.

- `DrawableCameraVirtual`
	- A drawable capable of drawing video out from a file. Useful for testing purposes.

### Basic Usage
```csharp
public void Main()
{
	// When using a physical camera
	var device = new CameraDevice(0);
	var drawableDevice = new DrawableCameraDevice(device);

	// When using a file (file is relative to application)
	var video = new CameraVirtual(@"test.mp4");
	var drawableVideo = new DrawableCameraVirtual(video);

	// When using a stream (usually obtained from resource stores)
	var videoStream = new CameraVirtual(Resources.GetStream(@"test.mp4"));
	var drawableVideoStream = new DrawableCameraVirtual(videoStream);
}
```

#### Additional Notes
The package only includes `OpenCVSharp4`. Please include its runtime libraries specific to the platform you are targeting.
See: [OpenCVSharp's Installation Guide](https://github.com/shimat/opencvsharp/blob/master/README.md#installation)

### License
This library is under the MIT License (see [LICENSE](./LICENSE.txt) / [tl;dr](https://tldrlegal.com/license/mit-license)).
Basically, you can do whatever you want as long as you include the original copyright and license notice.