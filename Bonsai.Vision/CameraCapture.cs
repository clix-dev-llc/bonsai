﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Threading;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Bonsai.Vision
{
    [Description("Produces a video sequence of images acquired from the specified camera index.")]
    public class CameraCapture : Source<IplImage>
    {
        IObservable<IplImage> source;
        readonly CapturePropertyCollection captureProperties = new CapturePropertyCollection();

        public CameraCapture()
        {
            source = Observable.Create<IplImage>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    using (var capture = Capture.CreateCameraCapture(Index))
                    {
                        foreach (var setting in captureProperties)
                        {
                            capture.SetProperty(setting.Property, setting.Value);
                        }
                        captureProperties.Capture = capture;
                        try
                        {
                            while (!cancellationToken.IsCancellationRequested)
                            {
                                var image = captureProperties.Capture.QueryFrame();
                                if (image == null)
                                {
                                    observer.OnCompleted();
                                    break;
                                }
                                else observer.OnNext(image.Clone());
                            }
                        }
                        finally { captureProperties.Capture = null; }
                    }
                });
            })
            .PublishReconnectable()
            .RefCount();
        }

        [Description("The index of the camera from which to acquire images.")]
        public int Index { get; set; }

        [Description("Specifies the set of capture properties assigned to the camera.")]
        public CapturePropertyCollection CaptureProperties
        {
            get { return captureProperties; }
        }

        public override IObservable<IplImage> Generate()
        {
            return source;
        }
    }
}
