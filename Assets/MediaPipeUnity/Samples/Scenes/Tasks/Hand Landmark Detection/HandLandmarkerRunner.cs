// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System;
using UnityEngine;
using TMPro;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine.Rendering;
using Mediapipe.Tasks.Components.Containers;
using System.Collections.Generic;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
  public class HandLandmarkerRunner : VisionTaskApiRunner<HandLandmarker>
  {
    [SerializeField] private HandLandmarkerResultAnnotationController _handLandmarkerResultAnnotationController;

    private Experimental.TextureFramePool _textureFramePool;

    public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();

    // Get this for index tip control!
    public Vector2 indexTipPosition = default;

    public override void Stop()
    {
      base.Stop();
      _textureFramePool?.Dispose();
      _textureFramePool = null;
    }

    protected override IEnumerator Run()
    {
      Debug.Log($"Delegate = {config.Delegate}");
      Debug.Log($"Running Mode = {config.RunningMode}");
      Debug.Log($"NumHands = {config.NumHands}");
      Debug.Log($"MinHandDetectionConfidence = {config.MinHandDetectionConfidence}");
      Debug.Log($"MinHandPresenceConfidence = {config.MinHandPresenceConfidence}");
      Debug.Log($"MinTrackingConfidence = {config.MinTrackingConfidence}");

      yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

      var options = config.GetHandLandmarkerOptions(config.RunningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnHandLandmarkDetectionOutput : null);
      taskApi = HandLandmarker.CreateFromOptions(options);
      var imageSource = ImageSourceProvider.ImageSource;

      yield return imageSource.Play();

      if (!imageSource.isPrepared)
      {
        Debug.LogError("Failed to start ImageSource, exiting...");
        yield break;
      }

      // Use RGBA32 as the input format.
      // TODO: When using GpuBuffer, MediaPipe assumes that the input format is BGRA, so maybe the following code needs to be fixed.
      _textureFramePool = new Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

      // NOTE: The screen will be resized later, keeping the aspect ratio.
      screen.Initialize(imageSource);

      SetupAnnotationController(_handLandmarkerResultAnnotationController, imageSource);

      var transformationOptions = imageSource.GetTransformationOptions();
      var flipHorizontally = transformationOptions.flipHorizontally;
      var flipVertically = transformationOptions.flipVertically;
      var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

      AsyncGPUReadbackRequest req = default;
      var waitUntilReqDone = new WaitUntil(() => req.done);
      var result = HandLandmarkerResult.Alloc(options.numHands);

      while (true)
      {
        if (isPaused)
        {
          yield return new WaitWhile(() => isPaused);
        }

        if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
        {
          yield return new WaitForEndOfFrame();
          continue;
        }

        // Copy current image to TextureFrame
        req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
        yield return waitUntilReqDone;

        if (req.hasError)
        {
          Debug.LogError($"Failed to read texture from the image source, exiting...");
          break;
        }

        var image = textureFrame.BuildCPUImage();
        switch (taskApi.runningMode)
        {
            case Tasks.Vision.Core.RunningMode.IMAGE:
                if (taskApi.TryDetect(image, imageProcessingOptions, ref result))
                {
                    // Access and log landmark 8's position
                    if (result.handLandmarks.Count > 8) // Ensure at least 9 landmarks exist
                    {
                      var landmark8 = result.handLandmarks[8];
                      //Debug.Log($"Landmark 8 Position: X = {landmark8.x}, Y = {landmark8.y}, Z = {landmark8.z}");
                    }
                    else
                    {
                      var landmark8 = result.handLandmarks[8];
                    }
                    _handLandmarkerResultAnnotationController.DrawNow(result);
                }
                else
                {
                    _handLandmarkerResultAnnotationController.DrawNow(default);
                }
                break;

            case Tasks.Vision.Core.RunningMode.VIDEO:
                if (taskApi.TryDetectForVideo(image, GetCurrentTimestampMillisec(), imageProcessingOptions, ref result))
                {
                    // Access and log landmark 8's position
                    if (result.handLandmarks.Count > 8) // Ensure at least 9 landmarks exist
                    {
                        var landmark8 = result.handLandmarks[8];
                        //Debug.Log($"Landmark 8 Position: X = {landmark8.x}, Y = {landmark8.y}, Z = {landmark8.z}");
                    }
                    else
                    {
                        var landmark8 = result.handLandmarks[8];
                    }
                    _handLandmarkerResultAnnotationController.DrawNow(result);
                }
                else
                {
                    _handLandmarkerResultAnnotationController.DrawNow(default);
                }
                break;

            case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                taskApi.DetectAsync(image, GetCurrentTimestampMillisec(), imageProcessingOptions);
                break;
        }


        textureFrame.Release();
      }
    }

    private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Image image, long timestamp)
    {
      var landmarks = result.handLandmarks;
      if (landmarks != null){
        var actualLandmarks = landmarks[0];
        indexTipPosition = new Vector3(actualLandmarks.landmarks[8].x,actualLandmarks.landmarks[8].y);
      }
      
      _handLandmarkerResultAnnotationController.DrawLater(result);
    }
  }
}
